// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.Extensions;

/// <summary>
/// 微信小程序 API 模块枚举（对齐公众号 <c>MpModule</c>；三段式注册的第一段）。
/// </summary>
/// <remarks>
/// <para>
/// <b>成员即 <c>RegistryGroupName</c></b>：与 <c>[HttpClientApi(RegistryGroupName = …)]</c> 同名，
/// 源生成器据此产出 <c>Add{域}WebApiHttpClient()</c>。
/// </para>
/// <para>
/// <b>令牌签发不经本枚举</b>：小程序复用公众号线令牌底座（<c>AddMpApp</c> 自动注册令牌客户端），
/// 故本枚举只列业务域。
/// </para>
/// <para>
/// <b>与 Work 线 <c>Security</c> 组同名是既有事实、且无害</b>：生成的注册类为各程序集内的
/// <c>internal static class HttpClientApiExtensions</c>（命名空间分别为 <c>Mud.Wechat.Work</c> 与
/// <c>Mud.Wechat.MiniProgram</c>）⇒ 命名空间不同、可见性为内部，不构成公开面歧义，
/// 也不会让两个建造者产生同名方法（各自挂在各自的 <c>ServiceBuilder</c> 上）。
/// </para>
/// </remarks>
public enum MiniProgramModule
{
    /// <summary>
    /// 登录与用户（5 端点：<c>code2Session</c> + 校验 / 重置 <c>session_key</c> + 获取手机号 + 支付后 <c>unionid</c>）。
    /// </summary>
    /// <remarks>
    /// 双接口同注册组：<c>IWxaAuthService</c> 4 端点带令牌 +
    /// <c>IWxaCode2SessionService</c> 1 端点免令牌（换会话走 appid + secret，不消费应用级令牌）。
    /// </remarks>
    Auth,

    /// <summary>
    /// 二维码 / 链接（8 端点：小程序码 3 + URL Link 2 + URL Scheme 2 + ShortLink 1）。
    /// </summary>
    /// <remarks>
    /// <b>双通道</b>：URL Link / Scheme / ShortLink 五端点响应为 JSON（生成管线）；
    /// 小程序码三端点响应为<b>图片二进制流</b>（失败时才是 JSON）⇒ 走独立请求形态
    /// <c>IWxaCodeService</c>（Content-Type 分支判错），不进 JSON 反序列化管线。
    /// </remarks>
    QrCodeLink,

    /// <summary>
    /// 内容安全（2 端点：文本同步审核 + 音视频异步审核）。
    /// </summary>
    /// <remarks><c>/wxa/img_sec_check</c> 官方文档已下架（硬 404），刻意不实现。</remarks>
    Security,

    /// <summary>
    /// 数据分析（9 端点：访问趋势 3 + 访问留存 3 + 用户画像 1 + 访问分布 1 + 访问页面 1）。
    /// </summary>
    /// <remarks>应答信封不成一形（逐页核验的事实）：趋势 / 分布 / 页面为 <c>{ref_date, list[]}</c>、留存为两个数组、画像为两个对象 ⇒ 逐端点各自 DTO。</remarks>
    DataAnalysis,
}
