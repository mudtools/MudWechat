// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Store;

/// <summary>
/// 新增门店（<c>POST /wxa/add_store</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表（逐页核验 2026-10-07）：<c>map_poi_id</c> / <c>pic_list</c> / <c>contract_phone</c> /
/// <c>hour</c> / <c>credential</c> / <c>card_id</c> 标必填；<c>company_name</c> / <c>qualification_list</c> /
/// <c>poi_id</c> 标非必填。
/// </para>
/// <para>
/// <b>官方必填性矛盾（照录，SDK 以可空承载、不本地拦截）</b>：① <c>card_id</c> 标必填但说明
/// 「如果不需要添加卡券，该参数可为空」；② <c>poi_id</c> 标非必填但说明「如果是迁移场景必须填」；
/// ③ <c>company_name</c> 标非必填但说明「不复用公众号主体则填具体主体名字」⇒ 条件必填。
/// </para>
/// <para>
/// <b>卡券约束（官方原文，勿弱化）</b>：<c>card_id</c>「目前仅开放支持<b>会员卡、买单、刷卡支付券</b>，
/// <b>不支持自定义 code</b>，需先去公众平台卡券后台创建 cardid」——本域只<b>透传</b>卡券 id，
/// 不建模卡券域、不消费卡券 <c>api_ticket</c>。
/// </para>
/// <para>
/// <b><c>qualification_list</c> 类型冲突口径</b>：官方字段表标 <c>array</c>，但说明为「支持 <b>0~5 个</b>
/// mediaid，例如 mediaid1 或 mediaid2」、示例为<b>单个字符串</b> ⇒ 按「容器形态冲突取字段表」建模为
/// <c>List&lt;string&gt;</c>（字段表为 schema 声明口径）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpAddStoreRequest
{
    /// <summary>获取或设置腾讯地图点位 id（官方 <c>map_poi_id</c>，必填；即 <c>search_map_poi</c> 返回的 <c>sosomap_poi_uid</c>）。</summary>
    [JsonPropertyName("map_poi_id")]
    public string MapPoiId { get; set; } = string.Empty;

    /// <summary>获取或设置门店图片（官方 <c>pic_list</c>，必填；一个 JSON 字符串，见 <see cref="MpStorePictureList"/>）。</summary>
    [JsonPropertyName("pic_list")]
    public string PictureList { get; set; } = string.Empty;

    /// <summary>获取或设置联系电话（官方 <c>contract_phone</c>，必填）。</summary>
    [JsonPropertyName("contract_phone")]
    public string ContractPhone { get; set; } = string.Empty;

    /// <summary>获取或设置营业时间（官方 <c>hour</c>，必填；格式 <c>11:11-12:12</c>）。</summary>
    [JsonPropertyName("hour")]
    public string Hour { get; set; } = string.Empty;

    /// <summary>获取或设置经营资质证件号（官方 <c>credential</c>，必填；唯一性由官方 85041 表达）。</summary>
    [JsonPropertyName("credential")]
    public string Credential { get; set; } = string.Empty;

    /// <summary>获取或设置主体名字（官方 <c>company_name</c>；复用公众号主体则为空，不复用则填具体主体名字）。</summary>
    [JsonPropertyName("company_name")]
    public string? CompanyName { get; set; }

    /// <summary>获取或设置卡券 id（官方 <c>card_id</c>；不需要添加卡券时可为空；仅支持会员卡 / 买单 / 刷卡支付券，不支持自定义 code）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>获取或设置相关证明材料（官方 <c>qualification_list</c>；临时素材 mediaid，支持 0~5 个；不复用公众号主体时才需要填）。</summary>
    [JsonPropertyName("qualification_list")]
    public List<string>? QualificationList { get; set; }

    /// <summary>获取或设置门店迁移用 poi_id（官方 <c>poi_id</c>；从门店管理迁移门店到门店小程序时需要填）。</summary>
    [JsonPropertyName("poi_id")]
    public string? PoiId { get; set; }
}
