// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.Customer;

/// <summary>
/// 批量获取客户详情请求体（<c>/cgi-bin/externalcontact/batch/get_by_user</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Customer")]
public class BatchGetCustomerDetailsRequest
{
    /// <summary>
    /// 获取或设置企业成员的 userid 列表（最多支持 100 个）。
    /// </summary>
    [JsonPropertyName("userid_list")]
    public List<string>? UseridList { get; set; }

    /// <summary>
    /// 获取或设置分页查询游标（由上一次调用返回，首次调用可不填）。
    /// </summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }

    /// <summary>
    /// 获取或设置返回的最大记录数（最大值 100，默认值 50，超过最大值时取最大值）。
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }
}
