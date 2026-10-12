// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Store;

/// <summary>
/// 更新门店信息（<c>POST /wxa/update_store</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表：<c>poi_id</c> / <c>pic_list</c> / <c>contract_phone</c> / <c>hour</c> / <c>card_id</c>。
/// 官方<b>未提供</b>任何资质 / 名称 / 地址类字段（更新门店基础资料无对应参数，照录）。
/// <c>card_id</c> 标必填但说明「不需要添加卡券时该参数可为空」——必填性矛盾照录。
/// </para>
/// <para>
/// <b>官方文档缺陷（照录）</b>：<c>map_poi_id</c> 出现在官方请求示例中，但请求参数表与 Query 表中
/// <b>均未定义</b>该字段（无类型、无必填性、无说明）⇒ SDK 不建模该字段。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpUpdateStoreRequest
{
    /// <summary>获取或设置门店 id（官方 <c>poi_id</c>，必填）。</summary>
    [JsonPropertyName("poi_id")]
    public string PoiId { get; set; } = string.Empty;

    /// <summary>获取或设置门店图片（官方 <c>pic_list</c>，必填；一个 JSON 字符串，见 <see cref="MpStorePictureList"/>）。</summary>
    [JsonPropertyName("pic_list")]
    public string PictureList { get; set; } = string.Empty;

    /// <summary>获取或设置联系电话（官方 <c>contract_phone</c>，必填）。</summary>
    [JsonPropertyName("contract_phone")]
    public string ContractPhone { get; set; } = string.Empty;

    /// <summary>获取或设置营业时间（官方 <c>hour</c>，必填；格式 <c>11:11-12:12</c>）。</summary>
    [JsonPropertyName("hour")]
    public string Hour { get; set; } = string.Empty;

    /// <summary>获取或设置卡券 id（官方 <c>card_id</c>；不需要添加卡券时可为空；约束同 <c>add_store</c>）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }
}
