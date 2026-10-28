// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Agent.Tools;
using Mud.Wechat.Abstractions.Contracts;

namespace Mud.Wechat.AI.Agent.Results;

/// <summary>
/// 微信生态统一信封读取器（跨七线）：把 <see cref="IWechatApiResponse"/> 解包为中立的
/// <see cref="ToolApiOutcome{T}"/>，作为工具执行链「成功投影 / 失败回填」的唯一入口。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何在共享层而非各线各写</b>：七线的响应基底 DTO 形态不同（企微恒带 <c>errcode</c>；公众号成功响应不带
/// <c>errcode</c>；支付 APIv3 无 <c>errcode</c>），但「判错面」已被叶层统一为
/// <see cref="IWechatApiResponse"/>（<c>ErrorCode == 0</c> 即成功）—— 解包规则因此可以一处收敛。
/// </para>
/// <para>
/// <b>码表纪律（不猜语义）</b>：码表本身不在这里，而在产品侧单源 <see cref="WechatApiErrorCatalog"/>——
/// 只收官方全局错误码文档中语义明确、且工具面确有恢复动作的码；未收录的码返回 <see langword="null"/>，
/// 中立层回落 <c>api/upstream_error</c>。猜测性码表比没有码表更危险（会把「参数错」告诉模型「稍后重试」）。
/// </para>
/// <para>
/// <b>错误正文含一行恢复建议</b>：分类读数同时以「建议：…」追加进 <see cref="ToolApiOutcome{T}.ErrorText"/>。
/// 这样<b>所有</b>既有执行器（只读 <c>ErrorText</c> 的形态）无需改动即获得可纠错文案——
/// 分类能力因此是「读取器的性质」，而不是「每个执行器记得拼的一步」。
/// </para>
/// <para>
/// <b>成功载荷为空</b>：<see cref="ToolApiOutcome{T}"/> 的成功语义要求载荷非空，故载荷为空时按失败回填
/// （<c>empty_payload</c>），避免模型把「空成功」当成「查到了空结果」。
/// </para>
/// </remarks>
public static class WechatApiResultReader
{
    /// <summary>
    /// 解包「信封即载荷」形态（响应 DTO 本身就是业务载荷，如 <c>WechatWorkResponse</c> 子类型）。
    /// </summary>
    /// <typeparam name="TResponse">响应类型（引用类型）。</typeparam>
    /// <param name="response">响应实例（不得为 <see langword="null"/>）。</param>
    /// <returns>解包结果。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="response"/> 为 <see langword="null"/>。</exception>
    public static ToolApiOutcome<TResponse> Read<TResponse>(TResponse response)
        where TResponse : class, IWechatApiResponse
    {
        if (response is null)
        {
            throw new ArgumentNullException(nameof(response));
        }

        return response.IsSuccess
            ? ToolApiOutcome<TResponse>.Success(response)
            : ToolApiOutcome<TResponse>.Fail(
                DescribeError(response),
                response.ErrorCode,
                WechatApiErrorCatalog.Classify(response.ErrorCode));
    }

    /// <summary>
    /// 解包「信封 + 独立载荷」形态（响应携带 data 字段，如 <c>WechatChatbotResponse&lt;T&gt;</c>）。
    /// </summary>
    /// <typeparam name="TData">业务载荷类型（引用类型）。</typeparam>
    /// <param name="envelope">判错面信封（不得为 <see langword="null"/>）。</param>
    /// <param name="data">业务载荷（成功时不得为 <see langword="null"/>）。</param>
    /// <returns>解包结果。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="envelope"/> 为 <see langword="null"/>。</exception>
    public static ToolApiOutcome<TData> Read<TData>(IWechatApiResponse envelope, TData? data)
        where TData : class
    {
        if (envelope is null)
        {
            throw new ArgumentNullException(nameof(envelope));
        }

        if (!envelope.IsSuccess)
        {
            return ToolApiOutcome<TData>.Fail(
                DescribeError(envelope),
                envelope.ErrorCode,
                WechatApiErrorCatalog.Classify(envelope.ErrorCode));
        }

        return data is null
            ? ToolApiOutcome<TData>.Fail("上游返回成功但业务载荷为空（empty_payload）", envelope.ErrorCode)
            : ToolApiOutcome<TData>.Success(data);
    }

    /// <summary>
    /// 构造可读错误文本（<c>errcode=…, errmsg=…</c>；errmsg 缺失时给中性描述），
    /// 已收录的码再追加一行「建议」（模型据此后可换路而不是原样重试）。
    /// </summary>
    private static string DescribeError(IWechatApiResponse response)
    {
        var message = string.IsNullOrWhiteSpace(response.ErrorMessage) ? "未提供错误信息" : response.ErrorMessage!.Trim();
        var text = $"errcode={response.ErrorCode}, errmsg={message}";
        var hint = WechatApiErrorCatalog.HintOf(response.ErrorCode);
        return hint.Length == 0 ? text : text + "\n" + hint;
    }
}
