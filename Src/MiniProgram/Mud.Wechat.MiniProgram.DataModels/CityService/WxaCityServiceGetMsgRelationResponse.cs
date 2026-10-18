// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.CityService;

/// <summary>
/// 查询用户是否开通长辈就医回包（<c>POST /cityservice/getmsgrelation</c>）。
/// </summary>
/// <remarks>
/// <para><b>官方示例字段名与字段表不一致（照录）</b>：字段表为 <c>errcode</c> / <c>errmsg</c>，
/// 返回示例写成 <c>err_code</c> / <c>err_msg</c> ⇒ 本仓按字段表（与通用信封一致）建模。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "CityService")]
public class WxaCityServiceGetMsgRelationResponse : WxaResponse
{
    /// <summary>是否订阅/开通，见 <see cref="WxaCityServiceMsgSubscriptionStatuses"/>。</summary>
    [JsonPropertyName("is_subscribed")]
    public int? IsSubscribed { get; set; }
}
