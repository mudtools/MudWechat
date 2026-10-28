// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.MerchantRiskManage;

/// <summary>
/// 商户违规通知回调地址的写入包体（<b>PUT 与 POST 两支共用</b>，官方两页字段表逐字相同）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：创建 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012471333"/>、
/// 修改 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012471330"/>，2026-10-14 逐字段核验。</para>
/// <para><b><c>notify_url</c> 官方标「选填」是本页最反直觉之处</b>：创建与修改两支都把它写成选填
/// ⇒ SDK 侧不做「必填」的客户端校验，但空白包体在语义上是「把回调地址置空」而非「不改动」，
/// 宿主须自觉显式赋值（守卫 MRB-B4 锁定该选填事实，防后来者"顺手补齐必填"造成契约漂移）。</para>
/// <para><b>两支共用本类型</b>：官方两页请求表只差标题，字段名/必填性/长度全同 ⇒ 按「表相同则共用」归并；
/// 与 <c>complaint-notifications</c> 族<b>不得</b>共用（那里字段名是 <c>url</c> 且为必填，应答另含 <c>mchid</c>）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantRiskManage")]
public class MerchantRiskManageViolationNotificationRequest
{
    /// <summary>通知地址（<c>notify_url</c>，官方标<b>选填</b> string(256)）：外网可访问、不得携带参数，公网域名须 HTTPS（专线可用 HTTP）。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }
}

/// <summary>
/// 商户违规通知回调地址的应答（<b>PUT / POST / GET 三支共用</b>，均为 HTTP 200 + 单字段 JSON）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：查询 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012471327"/>，2026-10-14 逐字段核验。</para>
/// <para><b>本域没有 204 的写支</b>：同路由的 DELETE 才是 204 无包体 ⇒ 「写操作一律 204」的想当然在本域不成立
/// （守卫 MRB-B1 逐支锁定「204 还是 JSON」）。</para>
/// <para><b>应答不回显 <c>mchid</c></b>：投诉通知族回 <c>{mchid, url}</c>，本族只回 <c>{notify_url}</c>
/// —— 两族应答形态差异是"不得共用 DTO"的直接依据。</para>
/// <para><b>未设置时查询返回空</b>：官方把 <c>notify_url</c> 在应答侧也标选填，且查询支另备
/// <c>404 NOT_FOUND</c> 错误码 ⇒ 「没配过」既可能是空字段也可能是 404，宿主两种都要接得住。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantRiskManage")]
public class MerchantRiskManageViolationNotificationResponse : WechatPayResponse
{
    /// <summary>当前生效的通知地址（<c>notify_url</c>，选填 string(256)）：未配置时官方可不返回该字段。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }
}
