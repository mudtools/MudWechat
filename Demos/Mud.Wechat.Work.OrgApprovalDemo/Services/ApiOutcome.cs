// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Http;
using Mud.Wechat.Work.DataModels;

namespace Mud.Wechat.Work.OrgApprovalDemo.Services;

/// <summary>
/// 端点响应的统一封装（方案 §4.4 末条）：官方业务错误 → 502 信封、参数错误 → 400。
/// </summary>
/// <remarks>
/// <para>
/// <b>区别于</b> Demo.Shared 的 <c>UseWechatApiErrorGate</c>：那个闸处理<b>异常</b>形态的错误
/// （令牌链与手工通道），本类处理 <c>errcode</c> 形态 —— 声明式生成端点在 <c>errcode != 0</c> 时
/// 仍是 HTTP 200，不逐端点判 <c>IsSuccess</c> 就等于把「官方拒了」当成「官方给了空数据」。
/// </para>
/// <para>
/// 信封字段与闸保持一致：<c>{ errorCode, errorMessage, requestUri, hint }</c>（前端一套解析逻辑通吃）。
/// </para>
/// </remarks>
internal static class ApiOutcome
{
    /// <summary>官方业务失败 → 502 + 统一信封。</summary>
    /// <param name="response">刚判过 <c>IsSuccess == false</c> 的官方响应。</param>
    /// <param name="route">本端点调用的官方路由（不含凭据 query）。</param>
    public static IResult BusinessFailure(WechatWorkResponse response, string route)
        => Results.Json(new
        {
            errorCode = response.ErrorCode,
            errorMessage = response.ErrorMessage,
            requestUri = route,
            hint = ErrorHints.Describe(response.ErrorCode),
        }, statusCode: StatusCodes.Status502BadGateway);

    /// <summary>参数不合法 → 400（前端就地标红，不弹全局错误）。</summary>
    public static IResult Invalid(string message) => Results.BadRequest(new { error = message });

    /// <summary>目标已被他人改动 → 409（模板 <c>baseVersion</c> 乐观锁；方案 §4.3）。</summary>
    public static IResult Conflict(string message)
        => Results.Json(new { error = message }, statusCode: StatusCodes.Status409Conflict);

    /// <summary>把任意整数夹进给定区间（官方 <c>limit</c>/<c>size</c> 都有上下界）。</summary>
    public static int Clamp(int value, int min, int max) => Math.Min(Math.Max(value, min), max);

    /// <summary>当前时间（Unix 秒）——全工程的「时间列一律 long」都由这一处产生。</summary>
    public static long NowUnix() => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
}

/// <summary>
/// 高频错误码的可读化：<b>只做映射、不复制码表</b>，未登记的码一律透传官方 <c>errmsg</c>。
/// </summary>
internal static class ErrorHints
{
    private static readonly Dictionary<int, string> Known = new()
    {
        [301115] = "模板已配置自定义打印格式，官方不支持经 API 修改（update_template 的系统限制，不是 Demo 缺陷）",
        [40001] = "access_token 无效（检查两个 secret 是否用错身份：写通讯录必须通讯录同步助手，读写模板必须审批应用）",
        [40003] = "userid 无效或不在应用可见范围内",
        [40011] = "部门不存在（user/create 填了不存在的部门 id 时官方会自动新建部门）",
        [40056] = "userid 在企业内已存在",
        [60003] = "缺少操作权限（自建应用须配置到「审批 - 可调用接口的应用」；通讯录写入须通讯录同步助手 secret）",
        [60011] = "无成员/部门访问权限（应用可见范围未覆盖该对象）",
        [301002] = "部门不存在或已被删除",
        [301025] = "部门层级超过上限（≤15 层）",
        [301026] = "不能删除含子部门或成员的部门",
        [40070] = "标签成员全部非法（三态返回的「全部非法」分支）",
        [45009] = "接口调用超过频率限制（模板读 600 次/分；全量同步请降并发）",
    };

    public static string? Describe(int errorCode)
        => Known.TryGetValue(errorCode, out var hint) ? hint : null;
}
