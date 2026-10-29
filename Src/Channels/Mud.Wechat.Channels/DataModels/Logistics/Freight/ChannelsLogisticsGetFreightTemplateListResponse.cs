// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 物流发货（Logistics）域「运费模板」类 DTO（merchant/addfreighttemplate、getfreighttemplatedetail、
// getfreighttemplatelist、updatefreighttemplate）。命名空间恒为 Mud.Wechat.Channels.DataModels.Logistics。

namespace Mud.Wechat.Channels.DataModels.Logistics;

/// <summary>获取运费模板列表（<c>merchant/getfreighttemplatelist</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsGetFreightTemplateListResponse : ChannelsResponse
{
    /// <summary>获取或设置运费模板 id 列表（官方 <c>template_id_list</c>）。</summary>
    [JsonPropertyName("template_id_list")]
    public List<string>? TemplateIdList { get; set; }
}
