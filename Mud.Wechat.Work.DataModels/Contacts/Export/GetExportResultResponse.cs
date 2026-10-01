// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Contacts.Export;

/// <summary>
/// 获取导出结果响应体（<c>/cgi-bin/export/get_result</c>；获取任务结果的调用身份需要与提交任务的一致）。
/// </summary>
/// <remarks>
/// 数据文件列表中的下载链接有效期 2 个小时、支持指定 Range 头分段下载；
/// 文件为提交任务时传入的 <c>encoding_aeskey</c> 对应 AES-256-CBC 加密的密文，解密由调用方自行完成。
/// 密文解密后的数据格式按导出类型与对应读取接口一致：导出成员 / 导出成员详情 / 导出标签成员为
/// <c>userlist</c>（导出标签成员另含 <c>tagname</c> 与 <c>partylist</c>），导出部门为 <c>department</c>。
/// </remarks>
public class GetExportResultResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置任务状态：0 - 未处理、1 - 处理中、2 - 完成、3 - 异常失败。
    /// </summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>
    /// 获取或设置数据文件列表（任务完成后有效）。
    /// </summary>
    [JsonPropertyName("data_list")]
    public List<ExportDataFile>? DataList { get; set; } = [];
}
