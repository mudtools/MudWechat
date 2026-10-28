// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.PromotionQrCode;

/// <summary>
/// 获取注册码请求体（<c>/cgi-bin/service/get_register_code</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PromotionQrCode")]
public class GetRegisterCodeRequest
{
    /// <summary>
    /// 获取或设置推广包 ID（最长 128 个字节，官方必填）。
    /// <para>在「服务商管理端-应用管理-推广二维码」创建的推广码详情中可查看。</para>
    /// </summary>
    [JsonPropertyName("template_id")]
    public string TemplateId { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置企业名称（官方选填）。
    /// <para>官方说明：传入本参数后，用户进入注册企业填写信息页面时，相应字段会自动填入表格。</para>
    /// </summary>
    [JsonPropertyName("corp_name")]
    public string? CorpName { get; set; }

    /// <summary>
    /// 获取或设置管理员姓名（官方选填）。
    /// <para>官方说明：传入本参数后，用户进入注册企业填写信息页面时，相应字段会自动填入表格。</para>
    /// </summary>
    [JsonPropertyName("admin_name")]
    public string? AdminName { get; set; }

    /// <summary>
    /// 获取或设置管理员手机号（官方选填）。
    /// <para>官方说明：传入本参数后，用户进入注册企业填写信息页面时，相应字段会自动填入表格。</para>
    /// </summary>
    [JsonPropertyName("admin_mobile")]
    public string? AdminMobile { get; set; }

    /// <summary>
    /// 获取或设置用户自定义的状态值（官方选填）。
    /// <para>官方限制：只支持英文字母和数字，最长为 128 字节。</para>
    /// <para>官方说明：若指定该参数，接口「查询注册状态」及「注册完成回调事件」会相应返回该字段值。</para>
    /// </summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>
    /// 获取或设置跟进人的 userid（官方选填）。
    /// <para>官方限制：必须是服务商所在企业的成员。</para>
    /// <para>官方说明：若配置该值，则由该注册码创建的企业，在服务商管理后台，该企业的报备记录会自动标注跟进人员为指定成员。</para>
    /// </summary>
    [JsonPropertyName("follow_user")]
    public string? FollowUser { get; set; }
}
