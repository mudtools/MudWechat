// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Shop;

/// <summary>
/// 店铺传播物料企业微信参数请求体（<c>h5url/get</c> 与 <c>taglink/get</c> 请求体字段集一致 ⇒ 共用；
/// 裁决理由见守卫 <c>ChannelsShopContractGuards</c> CS5）。
/// </summary>
/// <remarks>
/// 官方契约：两字段均选填，经「获取关联账号企微id」接口获取；带企微参数的链接 / 口令下单后，
/// 订单会在成交来源展示该企业。二维码端点额外多一个必填 <c>qrcode_type</c> ⇒ 独立 DTO 不共用。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Shop")]
public class ChannelsShopWecomLinkRequest
{
    /// <summary>获取或设置小店通过关联账号绑定的企业微信 id（官方 <c>wecom_corp_id</c>，选填）。</summary>
    [JsonPropertyName("wecom_corp_id")]
    public string? WecomCorpId { get; set; }

    /// <summary>获取或设置小店通过关联账号绑定的企业微信下的成员 id（官方 <c>wecom_user_id</c>，选填）。</summary>
    [JsonPropertyName("wecom_user_id")]
    public string? WecomUserId { get; set; }
}
