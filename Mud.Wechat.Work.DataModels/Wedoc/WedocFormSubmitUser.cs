// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 收集表统计信息中的已填写人（官方 <c>submit_users</c> 元素）。
/// </summary>
/// <remarks>
/// <para>
/// 官方业务限制：匿名填写时 <c>userid</c>、<c>user_name</c>、<c>tmp_external_userid</c> 均不返回；
/// 外部用户临时 id（tmp_external_userid）同一用户在不同收集表中不一致，须先经转换接口转为 external_userid 才能识别身份。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class WedocFormSubmitUser
{
    /// <summary>获取或设置企业内成员的 id（官方 <c>userid</c>），匿名填写不返回。</summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>获取或设置外部用户临时 id（官方 <c>tmp_external_userid</c>），匿名填写不返回；同一用户在不同收集表中该 id 不一致。</summary>
    [JsonPropertyName("tmp_external_userid")]
    public string? TmpExternalUserid { get; set; }

    /// <summary>获取或设置提交时间戳（官方 <c>submit_time</c>）。</summary>
    [JsonPropertyName("submit_time")]
    public ulong? SubmitTime { get; set; }

    /// <summary>获取或设置答案 id（官方 <c>answer_id</c>），可用于「读取收集表答案」接口。</summary>
    [JsonPropertyName("answer_id")]
    public ulong? AnswerId { get; set; }

    /// <summary>获取或设置填写人名字（官方 <c>user_name</c>），匿名填写不返回。</summary>
    [JsonPropertyName("user_name")]
    public string? UserName { get; set; }
}
