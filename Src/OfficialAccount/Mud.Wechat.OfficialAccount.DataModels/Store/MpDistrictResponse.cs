// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Store;

// ---------------------------------------------------------------- 获取省市区信息（GET /wxa/get_district）

/// <summary>
/// 获取省市区信息（<c>GET /wxa/get_district</c>）响应。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表：<c>status</c>（状态码）/ <c>message</c>（状态描述）/ <c>data_version</c>（数据版本）/
/// <c>result</c>（数据内容，<b>二维数组</b>，分别代表省、市、区信息）。
/// </para>
/// <para>
/// <b>继承 <see cref="MpResponse"/> 的理由（超集）</b>：成功响应<b>不含</b> <c>errcode</c>/<c>errmsg</c>
/// （仅 status/message），但官方错误码表列 <c>40001</c> ⇒ 失败时以 <c>errcode</c> 表达；两个面各自成立，
/// 故以超集承载（成功时 <c>IsSuccess</c> 缺省为真）。
/// </para>
/// <para>官方文档缺陷（照录）：<c>status</c> 为 <c>0</c> 却称「状态码」，未给出成功/失败取值定义。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpDistrictResponse : MpResponse
{
    /// <summary>获取或设置状态码（官方 <c>status</c>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置状态描述（官方 <c>message</c>）。</summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    /// <summary>获取或设置数据版本（官方 <c>data_version</c>）。</summary>
    [JsonPropertyName("data_version")]
    public string? DataVersion { get; set; }

    /// <summary>获取或设置数据内容（官方 <c>result</c>，<b>二维数组</b>：外层省 / 市 / 区三级，内层区域对象）。</summary>
    [JsonPropertyName("result")]
    public List<List<MpDistrictInfo>>? Result { get; set; }
}
