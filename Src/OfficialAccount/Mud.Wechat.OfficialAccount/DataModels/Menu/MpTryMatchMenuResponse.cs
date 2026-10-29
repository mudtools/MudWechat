// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Menu;

/// <summary>
/// 测试个性化菜单匹配结果响应（<c>tryMatchMenu</c>）。
/// </summary>
/// <remarks>
/// 官方响应仅有 <c>button</c>（菜单结构体数组，字段与创建菜单的按钮一致）——
/// <b>官方字段表未提供 <c>matchrule</c> 与 <c>menuid</c></b>，故 SDK 不建模它们（不据其他页「补全」）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Menu")]
public class MpTryMatchMenuResponse : MpResponse
{
    /// <summary>本次匹配命中的菜单按钮数组。</summary>
    [JsonPropertyName("button")]
    public List<MpMenuButton>? Button { get; set; }
}
