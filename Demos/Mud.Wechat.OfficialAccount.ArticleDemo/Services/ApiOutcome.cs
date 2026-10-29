// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Http;
using Mud.Wechat.OfficialAccount.DataModels;

namespace Mud.Wechat.OfficialAccount.ArticleDemo.Services;

/// <summary>
/// 端点响应的统一封装（方案 §4）：官方业务错误 → 502 信封、参数错误 → 400。
/// <b>区别于</b> Demo.Shared 的 <c>UseWechatApiErrorGate</c>（那个闸处理异常形态的错误，本类处理 errcode 形态）。
/// <para>
/// <b>为什么必须逐端点判 <c>IsSuccess</c></b>：声明式生成管线在 <c>errcode != 0</c> 时<b>仍是 HTTP 200</b>
/// 且反序列化成功 —— 不判错就等于把「微信拒了」当成「微信给了空数据」，前端会显示成「暂无草稿」这种误导。
/// 与 <c>UseWechatApiErrorGate</c> 的分工：令牌链/手工通道以异常承载错误（由闸映射 502），
/// 生成端点的 errcode 只有调用方自己知道，故由本类映射成<b>同一个信封</b>。
/// </para>
/// </summary>
internal static class ApiOutcome
{
    /// <summary>官方业务失败 → 502 + <c>{ errorCode, errorMessage, requestUri, hint }</c>。</summary>
    /// <param name="response">刚判过 <c>IsSuccess == false</c> 的官方响应。</param>
    /// <param name="route">本端点调用的官方路由（不含凭据 query，等价于闸侧脱敏后的 requestUri）。</param>
    public static IResult BusinessFailure(MpResponse response, string route)
        => Results.Json(new
        {
            errorCode = response.ErrorCode,
            errorMessage = response.ErrorMessage,
            requestUri = route,
            hint = ErrorHints.Describe(response.ErrorCode),
        }, statusCode: StatusCodes.Status502BadGateway);

    /// <summary>参数不合法 → 400（前端就地标红，见方案 §6.7）。</summary>
    public static IResult Invalid(string message) => Results.BadRequest(new { error = message });

    /// <summary>把任意整数夹进官方允许的区间。</summary>
    public static int Clamp(int value, int min, int max) => Math.Min(Math.Max(value, min), max);

    /// <summary>官方秒级时间戳（原样透传，时区由前端格式化，方案 §4.3 末尾）。</summary>
    public static long ToUnixSeconds(DateTimeOffset value) => value.ToUnixTimeSeconds();
}

/// <summary>
/// 高频错误码的可读化（方案 §10）：<b>只做映射、不复制码表</b>，未登记的码一律透传官方 <c>errmsg</c>，
/// Demo 不猜含义。
/// </summary>
internal static class ErrorHints
{
    private static readonly Dictionary<int, string> Known = new()
    {
        [40001] = "access_token 无效或已过期（组件会自愈重试一次，仍失败说明凭据或权限有问题）",
        [40007] = "media_id 无效或已随发布失效（发布成功后草稿被移除，官方生命周期）",
        [40009] = "图片超过大小限制（封面 10M / 正文图 1M）",
        [40013] = "AppId 无效（假凭据探针的预期结果）",
        [40114] = "index 越界（草稿只有 N 篇，请求的篇序号超出范围）",
        [41039] = "原文链接不合法",
        [42001] = "access_token 超时（组件会自愈重试一次）",
        [45166] = "正文不合法",
        [48001] = "账号未认证 / 无发布能力（freepublish 仅认证服务号可用）",
        [53503] = "该草稿未通过发布检查（缺封面、标题超长、正文为空等）",
        [53504] = "需前往公众平台官网使用该草稿（平台侧拦截，API 无法发布，不是 Demo 缺陷）",
        [53505] = "请先在官网手动保存成功后再发表",
        [53600] = "article_id 无效",
    };

    public static string? Describe(int errorCode)
        => Known.TryGetValue(errorCode, out var hint) ? hint : null;
}
