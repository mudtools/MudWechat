// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 物流发货（Logistics）域「电子面单」类 DTO（logistics/ewaybill/biz/*）。命名空间恒为
// Mud.Wechat.Channels.DataModels.Logistics。

namespace Mud.Wechat.Channels.DataModels.Logistics;

/// <summary>获取面单标准模板（<c>ewaybill/biz/template/config</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillGetTemplateConfigResponse : ChannelsResponse
{
    /// <summary>
    /// 获取或设置所有快递公司模板信息汇总（官方 <c>config</c>，键为快递公司 delivery_id，值为模板类型 → 模板项字典）。
    /// </summary>
    [JsonPropertyName("config")]
    public Dictionary<string, Dictionary<string, ChannelsLogisticsEwaybillTemplateConfig>>? Config { get; set; }
}
