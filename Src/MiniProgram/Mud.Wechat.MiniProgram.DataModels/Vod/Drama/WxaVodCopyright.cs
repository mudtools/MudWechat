// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 版权保护信息（官方 <c>Body.copyright</c>）。
/// </summary>
/// <remarks>
/// <para><b>共用裁决</b>：<c>auditdrama</c> 与 <c>modifydramabasicinfo</c> 的版权子表字段名完全相同（两页必填标记不同：提审侧 <c>copyright_verification</c>/<c>proof_of_production</c> 必填，修改侧选填）⇒ 共用本类型，差异靠可空属性 + 宿主按场景赋值承载。</para>
/// <para><b><c>copyright_verification</c> 官方类型列作 <c>string</c>、值域却写成数字 1/2（照录原文，两侧页面一致）</b>⇒ 属性与常量类都用 <c>string</c>；官方两页代码示例均未出现该字段，无从以示例推翻表格。<b>若宿主侧出现「参数类型不匹配」类报错，须回到本页复核该字段线格式</b>（该风险已留档，不得静默改成数字）。</para>
/// <para><b>2026-07-28 12:00 起 <c>apply_for_copyright_protection</c> 默认传 1 且不支持修改</b>（官方原文），此时 <c>proof_of_production</c> 转为必填、最多 10 个。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/auditdrama/api_auditdrama.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vod")]
public class WxaVodCopyright
{
    /// <summary>copyright_role（官方类型 <c>number</c>，<b>必填</b>）：提审主体身份，1. 剧目制作方; 2. 授权播出方或版权方。</summary>
    [JsonPropertyName("copyright_role")]
    public long? CopyrightRole { get; set; }

    /// <summary>apply_for_copyright_protection（官方类型 <c>number</c>，<b>必填</b>）：是否申请版权保护，0. 不申请; 1. 申请。不申请时必须上传权利声明/播放授权材料。 2026年7月28日12:00起，默认传1且不支持修改。</summary>
    [JsonPropertyName("apply_for_copyright_protection")]
    public long? ApplyForCopyrightProtection { get; set; }

    /// <summary>copyright_verification（官方类型 <c>string</c>，<b>必填</b>）：版权验证方式：1. 基于版权证明材料; 2. 基于版权授权关系。 （2024年12月30日00:00起，apply_for_copyright_protection=1时必填。）</summary>
    [JsonPropertyName("copyright_verification")]
    public string? CopyrightVerification { get; set; }

    /// <summary>proof_of_production（官方类型 <c>array</c>，<b>必填</b>）：剧目制作证明材料临时 material_id，最多支持上传 4 个。（apply_for_copyright_protection=1时必填。 2024年12月30日00:00起，apply_for_copyright_protection=1且copyright_verification=2时无需填写。 ） 注： 剧目类型=真人，需提供以下任一材料： 1. 剧目制作合同； 2. 作品登记证书； 3. 两份或以上主创人员的聘用合同（如：导演合同+主演合同）。 剧目类型=漫剧/数字真人，支持上述材料，或以下任一材料： 1. 剧本采买合同； 2. 后期制作工程文件，如：剪辑、特效图层工程等； 3. 可信时间戳认证证书。 2026年7月28日12:00起，作如下调整： 调整为必填，最多支持上传10个。 剧目类型=真人，请提供以下任一材料：1. 剧目制作合同，需明确作品权利归属，若存在多版权方，请在【制作方名称】处填写完整权利主体名称，主体间以顿号（、）分隔； 2. 作品登记证书，作品类别需为“视听”； 3. 两份或以上主创人员的聘用合同（如：导演合同+主演合同）。 剧目类型=漫剧/数字真人，请同时提供以下材料： 1. 至少4张不同剧集的后期制作展示图，需包含画面、配音、字幕等合理分轨及剪辑点。查看示例 2. 6选1： （1）剧目制作合同，需明确作品权利归属，若存在多版权方，请在【制作方名称】处填写完整权利主体名称，主体间以顿号（、）分隔； （2）无AI创作剧目：至少4张剧目内不同角色/场景，对应的原始素材截图/分镜草稿/角色原画/场景原画等；查看示例 （3）AI创作剧目：至少4张同剧目内不同主要角色或场景，对应的提示词、操作过程及对应生成结果的截图；查看示例 （4）作品登记证书，作品类别需为“视听”；查看示例 （5）可信时间戳认证证书/作品存证证书，“保护类型/作品类型”需为“视听作品”； 查看示例 （6）剧本采买/改编/转让合同，需明确基于剧本创作/改编的作品权属。</summary>
    [JsonPropertyName("proof_of_production")]
    public List<string>? ProofOfProduction { get; set; }

    /// <summary>purchase_or_broadcast_authorization_certificate（官方类型 <c>array</c>）：版权采买/播出授权证明材料临时 material_id，最多支持上传 4 个。 提审身份非剧目制作方时请上传： 1. 版权采买证明材料 (提审身份为版权方/授权播出方时需提供)； 2. 播出授权材料 (提审身份为授权播出方时需提供) （copyright_verification=1时选填，copyright_verification=2时无需填写。）</summary>
    [JsonPropertyName("purchase_or_broadcast_authorization_certificate")]
    public List<string>? PurchaseOrBroadcastAuthorizationCertificate { get; set; }
}
