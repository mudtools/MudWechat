// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 添加数据表响应体（<c>/cgi-bin/wedoc/smartdoc/add_smartsheet</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class AddSmartDocDataTableResponse : WechatWorkResponse
{
    /// <summary>获取或设置添加成功后返回的数据表信息（官方 <c>info</c>），包含系统生成的 <c>block_id</c>。</summary>
    [JsonPropertyName("info")]
    public SmartDocDataTableInfo? Info { get; set; }
}
