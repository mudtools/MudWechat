// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 获取数据源响应体（<c>/cgi-bin/wedoc/smartdoc/get_smartsheet_info</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class GetSmartDocDataSourceResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置智能文档所绑定的数据表 docid（官方 <c>ss_docid</c>）。
    /// <para>官方行为：若该智能文档已绑定数据表则返回已有数据表的 docid；若尚未绑定则自动创建数据表并返回新创建的 docid。</para>
    /// </summary>
    [JsonPropertyName("ss_docid")]
    public string? SsDocid { get; set; }
}
