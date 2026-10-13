// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 「剧目提审」请求体（<c>POST /wxa/sec/vod/auditdrama</c>）：本域字段面最宽的一支（24 参数）。
/// </summary>
/// <remarks>
/// <para><b>首次提审与重新提审的必填面不同（官方原文）</b>：剧目信息与审核材料在首次提审时必填，重新提审时根据是否需要修改选填；<c>drama_id</c> 首次不填、重新提审必填；<c>media_count</c> 重新提审可不填，填了必须与第一次一致 ⇒ SDK 一律做成可空属性，必填面由宿主按场景掌握。</para>
/// <para><b>图片类参数一律用 <c>material_id</c> 而非 URL</b>（官方原文）：临时图片 material_id 由「新增临时素材」接口得到，它对应临时素材接口的 <c>media_id</c>；<b>调用素材上传的账号必须与提审账号同一个</b>，否则提审时无法识别素材 id。</para>
/// <para><b>备案机制调整留档（官方原文）</b>：2024-05-27 起制作成本 30 万元以下的微短剧（2026-01-14 起调整为 100 万元以下）需上传《成本配置比例情况报告》，剧目经平台审核后由平台下发备案号（备案号仅适用微信小程序平台）。</para>
/// <para><b><c>authorized_material_id</c> 官方标注 2026-07-28 12:00 起弃用</b>（后续无须上传）⇒ 属性保留但不再必填。</para>
/// <para><b>接口变更日志（官方页留档）</b>：2026-07-21 新增「AI 权属证明」与「其他平台证明材料」两字段；2026-04-13 剧目类型新增「数字真人」。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/auditdrama/api_auditdrama.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vod")]
public class WxaVodAuditDramaRequest
{
    /// <summary>drama_id（官方类型 <c>number</c>，<b>必填</b>）：剧目id，首次提审不需要填该参数，重新提审时必填</summary>
    [JsonPropertyName("drama_id")]
    public long? DramaId { get; set; }

    /// <summary>name（官方类型 <c>string</c>，<b>必填</b>）：剧目名称，首次提审时必填，重新提审时根据是否需要修改选填。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>media_count（官方类型 <c>number</c>，<b>必填</b>）：剧集数目。首次提审时必填， 重新提审时可不填，如要填写也要和第一次提审时一样。</summary>
    [JsonPropertyName("media_count")]
    public long? MediaCount { get; set; }

    /// <summary>media_id_list（官方类型 <c>numarray</c>，<b>必填</b>）：剧集媒资media_id列表。首次提审时必填，而且元素个数必须与media_count一致。重新提审时为可选，如果剧集有内容有变化，可以通过新的列表替换未通过的剧集（推荐使用replace_media_list进行替换，避免顺序和原列表不一致）。</summary>
    [JsonPropertyName("media_id_list")]
    public List<long>? MediaIdList { get; set; }

    /// <summary>description（官方类型 <c>string</c>，<b>必填</b>）：剧目简介，可填写200个字符。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>recommendations（官方类型 <c>string</c>）：剧目推荐语，可填写30个字符。</summary>
    [JsonPropertyName("recommendations")]
    public string? Recommendations { get; set; }

    /// <summary>cover_material_id（官方类型 <c>string</c>，<b>必填</b>）：剧目海报临时material_id。首次提审时必填，重新提审时根据是否需要修改选填。</summary>
    [JsonPropertyName("cover_material_id")]
    public string? CoverMaterialId { get; set; }

    /// <summary>promotion_poster_material_id（官方类型 <c>string</c>）：推广海报临时material_id。</summary>
    [JsonPropertyName("promotion_poster_material_id")]
    public string? PromotionPosterMaterialId { get; set; }

    /// <summary>producer（官方类型 <c>string</c>，<b>必填</b>）：剧目制作方 。首次提审时必填，重新提审时根据是否需要修改选填。</summary>
    [JsonPropertyName("producer")]
    public string? Producer { get; set; }

    /// <summary>authorized_material_id（官方类型 <c>string</c>）：2026年7月28日12:00起弃用，后续无须上传。 不申请版权保护时必填，权利声明/播放授权材料material_id。 需上传《权利声明及不侵权承诺函》 注：(1) 剧目制作方/版权方与账号主体一致时，请上传附件1（需签章）； (2) 剧目制作方/版权方与账号主体不一致时，请上传附件2或完整的播放授权材料（需签章）。</summary>
    [JsonPropertyName("authorized_material_id")]
    public string? AuthorizedMaterialId { get; set; }

    /// <summary>qualification_type（官方类型 <c>number</c>，<b>必填</b>）：剧目资质：1-取得《网络剧片发行许可证》或重点节目备案号；2-未取得《网络剧片发行许可证》或重点节目备案，且制作成本小于100万元 注： 1、 （1）剧目资质=1：需上传“网络剧片发行许可证”或“广电备案系统截图”，平台会在视频播放环节展示备案号水印； （2）剧目资质=2：制作成本在100万以内，需上传《成本配置比例情况报告》。剧目经平台审核后由平台下发备案号（备案号仅适用微信小程序平台）并在视频播放环节展示备案号水印。 2、2024年5月27日前发起提审的剧目可支持修改，且同一剧目仅支持修改一次。</summary>
    [JsonPropertyName("qualification_type")]
    public long? QualificationType { get; set; }

    /// <summary>registration_number（官方类型 <c>string</c>）：剧目备案号，当qualification_type=1时必填。根据提供的剧目资质证明文件填写对应的网络剧片发行许可证编号或剧目备案号。如：(沪)网剧审字(2023)第001号 或 V123456788888888</summary>
    [JsonPropertyName("registration_number")]
    public string? RegistrationNumber { get; set; }

    /// <summary>qualification_certificate_material_id（官方类型 <c>string</c>）：剧目资质证明文件，当qualification_type=1时必填。请提供网络剧片发行许可证或广电备案系统截图。查看截图示例</summary>
    [JsonPropertyName("qualification_certificate_material_id")]
    public string? QualificationCertificateMaterialId { get; set; }

    /// <summary>cost_commitment_letter_material_id（官方类型 <c>string</c>）：《成本配置比例情况报告》material_id，当qualification_type=2时必填。2026年1月14日00:00起提审需使用新模板。点击下载旧模板，点击下载新模板</summary>
    [JsonPropertyName("cost_commitment_letter_material_id")]
    public string? CostCommitmentLetterMaterialId { get; set; }

    /// <summary>cost_of_production（官方类型 <c>number</c>）：剧目制作成本（单位：万元），当qualification_type=2时必填。请填写“1-99” 的整数（如非整数将截断取整），数值需与《成本配置比例情况报告》中对应剧目制作成本一致。</summary>
    [JsonPropertyName("cost_of_production")]
    public long? CostOfProduction { get; set; }

    /// <summary>expedited（官方类型 <c>number</c>）：填1表示审核加急，0或不填为不加急。每天有5次加急机会。该字段在首次提审时才有效，重新提审时会沿用首次提审时的属性，重新提审不会扣次数。最终是否为加急单，可以根据drama_info_list.expedited属性(1表示审核加急，0或空为非加急审核)判断</summary>
    [JsonPropertyName("expedited")]
    public long? Expedited { get; set; }

    /// <summary>actor_list（官方类型 <c>object</c>）：演员信息，需填写2-5位演员。当drama_type=2时必填，当drama_type=1和3时无需填写。</summary>
    [JsonPropertyName("actor_list")]
    public WxaVodActorList? ActorList { get; set; }

    /// <summary>other_material_material_id（官方类型 <c>string</c>）：其他材料material_id，如涉及互动短剧，请上传完整剧情走线示意图。</summary>
    [JsonPropertyName("other_material_material_id")]
    public string? OtherMaterialMaterialId { get; set; }

    /// <summary>replace_media_list（官方类型 <c>objarray</c>）：用于重新提审时替换审核不通过的剧集。</summary>
    [JsonPropertyName("replace_media_list")]
    public List<WxaVodReplaceMediaItem>? ReplaceMediaList { get; set; }

    /// <summary>copyright（官方类型 <c>object</c>，<b>必填</b>）：版权保护相关。</summary>
    [JsonPropertyName("copyright")]
    public WxaVodCopyright? Copyright { get; set; }

    /// <summary>drama_type（官方类型 <c>number</c>，<b>必填</b>）：剧目类型，1-漫剧；2-真人。新增3-数字真人（2026年4月20日11:00起生效） 注： 1.漫剧：指基于漫画、动画或绘图风格制作的剧目，不涉及真人出演。提审流程无需上传对应的演职人员信息。 2.真人：指由真实演员出演、使用实景拍摄或特效辅助制作的剧目。提审流程需上传对应的演职人员信息。 3.数字真人：指基于计算机图形技术与人工智能算法生成的、具有逼真人类外观及行为表现的虚拟角色出演的剧目。提审流程无需上传对应的演职人员信息。 4.漫剧和数字真人均无需填写「actor_list-演员信息」字段</summary>
    [JsonPropertyName("drama_type")]
    public long? DramaType { get; set; }

    /// <summary>content_declared（官方类型 <c>number</c>）：AI内容声明，1-视频包含AI生成的内容；0或不传-视频不包含AI生成的内容</summary>
    [JsonPropertyName("content_declared")]
    public long? ContentDeclared { get; set; }

    /// <summary>ai_software_ownership_proof（官方类型 <c>string</c>）：AI制作证明材料临时 material_id，当content_declared=1时必填。请上传使用AI软件生成内容的证明截图。（2026年7月28日12:00起生效）</summary>
    [JsonPropertyName("ai_software_ownership_proof")]
    public string? AiSoftwareOwnershipProof { get; set; }

    /// <summary>other_platform_publication_proof（官方类型 <c>array</c>）：其他平台发布证明临时 material_id，最多支持上传4个。如剧目已在抖音/快手等第三方平台发布，请补充上架证明，如作品管理界面截图或版权中心登记截图等。（2026年7月28日12:00起生效）</summary>
    [JsonPropertyName("other_platform_publication_proof")]
    public List<string>? OtherPlatformPublicationProof { get; set; }
}
