// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.CorpGroup.ChainContacts;

/// <summary>
/// 获取导入任务结果响应体（<c>/cgi-bin/corpgroup/getresult</c>；只能查询已提交过的历史任务，并发限制 5）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "ChainContacts")]
public class GetChainImportResultResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置任务状态：1 - 任务开始，2 - 任务进行中，3 - 任务已完成。
    /// </summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>
    /// 获取或设置详细的处理结果（任务完成后此字段有效）。
    /// </summary>
    [JsonPropertyName("result")]
    public ChainImportResult? Result { get; set; }
}
