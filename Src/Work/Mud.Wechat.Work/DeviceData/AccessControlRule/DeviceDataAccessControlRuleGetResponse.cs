// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.DeviceData;

/// <summary>
/// 获取门禁规则响应体（<c>/cgi-bin/devicedata/get_accesscontrol_rule</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DeviceData")]
public class DeviceDataAccessControlRuleGetResponse
{
    /// <summary>获取或设置门禁规则列表容器（官方 pass_rule，响应侧为「容器 + items」形态，与添加 / 修改请求侧的单对象形态同名不同形）。</summary>
    [JsonPropertyName("pass_rule")]
    public DeviceDataAccessControlRuleList? PassRuleList { get; set; }

    /// <summary>获取或设置远程门禁规则列表容器（官方 remote_pass_rule，容器形态同 pass_rule）。</summary>
    [JsonPropertyName("remote_pass_rule")]
    public DeviceDataAccessControlRuleList? RemotePassRuleList { get; set; }
}
