// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.Abstractions;

/// <summary>
/// 微信小程序业务错误码（HTTP 200 + <c>errcode</c> 通道，与公众号同形）。
/// </summary>
/// <remarks>
/// <para>
/// <b>只登记本线端点实际会返回的码</b>（逐页核验；数据分析域多数页面仅列 <c>-1</c> 与 <c>40001</c>，
/// 通用码表另有 <c>40013</c>/<c>41002</c> 等，本线未引用者不登记，避免制造无消费点常量）。
/// </para>
/// <para>
/// <b>令牌失效码语义</b>：<c>40001</c>（<c>access_token</c> 无效 / 非最新）由公众号线
/// <c>MpTokenInvalidationDetector</c> 识别后交恢复链路「失效缓存 → 刷新 → 重试」——
/// 小程序<b>复用同一令牌域与同一判定器</b>，故此处只作常量登记，<b>不另建判定器</b>（MP-X2）。
/// </para>
/// </remarks>
public static class WxaErrorCodes
{
    /// <summary>成功。</summary>
    public const int Success = 0;

    /// <summary>系统繁忙（官方 <c>-1</c>，请稍候再试）。</summary>
    public const int SystemError = -1;

    /// <summary><c>access_token</c> 无效 / 非最新（走令牌自愈）。</summary>
    public const int InvalidCredential = 40001;

    /// <summary>不合法的 <c>js_code</c>（<c>code2Session</c>；已使用或过期）。</summary>
    public const int InvalidCode = 40029;

    /// <summary>调用频率超限（请降低频率后重试）。</summary>
    public const int FrequentLimit = 45011;

    /// <summary>高风险用户（官方 <c>40226</c>，建议直接拦截该用户）。</summary>
    public const int RiskUserRejected = 40226;

    /// <summary>数据格式错误（参数格式不合法）。</summary>
    public const int DataFormatError = 47001;

    /// <summary>空 POST 数据（请求体缺失）。</summary>
    public const int EmptyPostData = 44002;

    /// <summary>接口无权限（未开通或账号类型不符）。</summary>
    public const int ApiUnauthorized = 48001;

    /// <summary>用户登录态签名校验失败（<c>checksession</c> / <c>resetusersessionkey</c>）。</summary>
    public const int SignatureCheckFailed = 87009;

    /// <summary>订单不存在或不匹配（<c>getpaidunionid</c> 前置订单校验）。</summary>
    public const int OrderNotFound = 9300501;

    /// <summary>内容含违法违规信息（内容安全：文本 / 音视频审核命中风险）。</summary>
    public const int RiskyContent = 87014;

    /// <summary>无权限调用内容安全接口（官方 <c>43104</c>，须先开通内容安全能力）。</summary>
    public const int SecurityApiPermissionDenied = 43104;
}
