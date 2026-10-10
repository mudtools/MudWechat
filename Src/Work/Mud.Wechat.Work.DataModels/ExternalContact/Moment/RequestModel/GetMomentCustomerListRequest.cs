// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.Moment;

/// <summary>
/// 获取客户朋友圈发表时选择的可见范围请求体（<c>/cgi-bin/externalcontact/get_moment_customer_list</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Moment")]
public class GetMomentCustomerListRequest
{
    /// <summary>
    /// 获取或设置朋友圈 id（官方必填）。
    /// </summary>
    [JsonPropertyName("moment_id")]
    public string? MomentId { get; set; }

    /// <summary>
    /// 获取或设置朋友圈发表成员的 userid（官方必填；
    /// 企业发表的朋友圈用「获取客户朋友圈企业发表的列表」获取，个人发表的朋友圈即创建人 userid）。
    /// </summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>
    /// 获取或设置用于分页查询的游标，由上一次调用返回，首次调用不填。
    /// </summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }

    /// <summary>
    /// 获取或设置返回的最大记录数（最大值为 1000，默认为 500）。
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }
}
