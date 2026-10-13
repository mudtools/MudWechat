// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 「修改剧目基本信息」请求体（<c>POST /wxa/sec/vod/modifydramabasicinfo</c>，15 参数）。
/// </summary>
/// <remarks>
/// <para><b>前置态（官方原文）</b>：剧目必须已经审核通过；审核完成后会发送事件通知。</para>
/// <para><b>演员信息是「全量覆盖」而非增量</b>（官方原文「如果需要修改，请填写所有的演员信息」）⇒ 只传增量会丢其余演员。</para>
/// <para><b>本页 <c>qualification_type</c> 的说明列仍写「制作成本小于30万元」、注释列又写「100万以内」</b>（同一单元格自相矛盾，照录不采信 ⇒ 以 <c>auditdrama</c> 页 2026-01-14 的调整为准）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/auditdrama/api_submitmodifydramabasicinforeq.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vod")]
public class WxaVodModifyDramaBasicInfoRequest
{
    /// <summary>drama_id（官方类型 <c>number</c>，<b>必填</b>）：剧目id</summary>
    [JsonPropertyName("drama_id")]
    public long? DramaId { get; set; }

    /// <summary>description（官方类型 <c>string</c>）：剧目简介，支持填写200个字符。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>cover_material_id（官方类型 <c>string</c>）：剧目海报临时material_id。</summary>
    [JsonPropertyName("cover_material_id")]
    public string? CoverMaterialId { get; set; }

    /// <summary>recommendations（官方类型 <c>string</c>）：剧目推荐语。</summary>
    [JsonPropertyName("recommendations")]
    public string? Recommendations { get; set; }

    /// <summary>promotion_poster_material_id（官方类型 <c>string</c>）：推广海报临时material_id。</summary>
    [JsonPropertyName("promotion_poster_material_id")]
    public string? PromotionPosterMaterialId { get; set; }

    /// <summary>alternate_name（官方类型 <c>string</c>）：备用剧名。</summary>
    [JsonPropertyName("alternate_name")]
    public string? AlternateName { get; set; }

    /// <summary>actor_list（官方类型 <c>object</c>）：演员信息。如果需要修改，请填写所有的演员信息。</summary>
    [JsonPropertyName("actor_list")]
    public WxaVodActorList? ActorList { get; set; }

    /// <summary>qualification_type（官方类型 <c>number</c>）：剧目资质：1-取得《网络剧片发行许可证》或重点节目备案号；2-未取得《网络剧片发行许可证》或重点节目备案，且制作成本小于30万元 注： 1、 （1）剧目资质=1：需上传“网络剧片发行许可证”或“广电备案系统截图”，平台会在视频播放环节展示备案号水印； （2）剧目资质=2：制作成本在100万以内，需上传《成本配置比例情况报告》。剧目经平台审核后由平台下发备案号（备案号仅适用微信小程序平台）并在视频播放环节展示备案号水印。 2、2024年5月27日前发起提审的剧目可支持修改，且同一剧目仅支持修改一次。</summary>
    [JsonPropertyName("qualification_type")]
    public long? QualificationType { get; set; }

    /// <summary>registration_number（官方类型 <c>string</c>）：剧目备案号，当qualification_type=1时必填。根据提供的剧目资质证明文件填写对应的网络剧片发行许可证编号或剧目备案号。如：(沪)网剧审字(2023)第001号 或 V123456788888888</summary>
    [JsonPropertyName("registration_number")]
    public string? RegistrationNumber { get; set; }

    /// <summary>qualification_certificate_material_id（官方类型 <c>string</c>）：剧目资质证明文件，当qualification_type=1时必填。请提供网络剧片发行许可证或广电备案系统截图。查看截图示例</summary>
    [JsonPropertyName("qualification_certificate_material_id")]
    public string? QualificationCertificateMaterialId { get; set; }

    /// <summary>cost_of_production（官方类型 <c>number</c>）：剧目制作成本（单位：万元），当qualification_type=2时必填。请填写“1-99” 的整数（如非整数将截断取整），数值需与《成本配置比例情况报告》中对应剧目制作成本一致。</summary>
    [JsonPropertyName("cost_of_production")]
    public long? CostOfProduction { get; set; }

    /// <summary>cost_commitment_letter_material_id（官方类型 <c>string</c>）：《成本配置比例情况报告》material_id，当qualification_type=2时必填。2026年1月14日00:00起提审需使用新模板。点击下载旧模板，点击下载新模板</summary>
    [JsonPropertyName("cost_commitment_letter_material_id")]
    public string? CostCommitmentLetterMaterialId { get; set; }

    /// <summary>other_material_material_id（官方类型 <c>string</c>）：其他材料material_id，如涉及互动短剧，请上传完整剧情走线示意图。</summary>
    [JsonPropertyName("other_material_material_id")]
    public string? OtherMaterialMaterialId { get; set; }

    /// <summary>producer（官方类型 <c>string</c>）：剧目制作方。apply_for_copyright_protection =1时必填，否则无需填写。</summary>
    [JsonPropertyName("producer")]
    public string? Producer { get; set; }

    /// <summary>copyright（官方类型 <c>object</c>）：版权保护相关。apply_for_copyright_protection =1时必填，否则无需填写 。</summary>
    [JsonPropertyName("copyright")]
    public WxaVodCopyright? Copyright { get; set; }
}
