// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Kf;

/// <summary>
/// 获取客服账号列表请求体（<c>/cgi-bin/kf/account/list</c>）。
/// <para>
/// 分页形态为 offset + limit：当返回的账号数量小于指定的 limit 时，表示已无更多数据，应终止获取。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class GetKfAccountListRequest
{
    /// <summary>
    /// 获取或设置分页偏移量（默认为 0，以 0 为起点）。
    /// </summary>
    [JsonPropertyName("offset")]
    public int? Offset { get; set; }

    /// <summary>
    /// 获取或设置预期请求的数据量（默认为 100，取值范围 1~100）。
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }
}
