// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Category;

/// <summary>类目资质详情（官方 <c>qua</c> 对象；<c>cert_group_list</c> 仅 <c>cats</c> 旧树版本携带）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryQua
{
    /// <summary>获取或设置资质 ID（官方 <c>qua_id</c>）。</summary>
    [JsonPropertyName("qua_id")]
    public long? QuaId { get; set; }

    /// <summary>获取或设置该类目是否需要申请才能用（官方 <c>need_to_apply</c>）。</summary>
    [JsonPropertyName("need_to_apply")]
    public bool? NeedToApply { get; set; }

    /// <summary>获取或设置资质描述信息（官方 <c>tips</c>）。</summary>
    [JsonPropertyName("tips")]
    public string? Tips { get; set; }

    /// <summary>获取或设置申请此类目时是否必须提交资质证明（官方 <c>mandatory</c>）。</summary>
    [JsonPropertyName("mandatory")]
    public bool? Mandatory { get; set; }

    /// <summary>
    /// 获取或设置证照信息列表（官方 <c>cert_group_list</c>，仅 <c>cats</c> 旧树版本携带）——
    /// 每个类目有多个证照组要求，须提交全部证照组；证照组内的证照选一个填写提交即可。
    /// </summary>
    [JsonPropertyName("cert_group_list")]
    public List<ChannelsCategoryCertGroup>? CertGroupList { get; set; }
}
