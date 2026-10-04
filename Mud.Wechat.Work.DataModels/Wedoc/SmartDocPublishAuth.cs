// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 发布可见范围成员项（官方 <c>auth_list</c> 元素；发布智能文档请求与修改发布页可查看范围请求共用）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class SmartDocPublishAuth
{
    /// <summary>
    /// 获取或设置成员类型（官方 <c>type</c>，必填）。
    /// 官方取值：<c>1</c> 企业成员、<c>2</c> 部门。
    /// </summary>
    [JsonPropertyName("type")]
    public uint? Type { get; set; }

    /// <summary>获取或设置企业成员的 userid（官方 <c>userid</c>），<c>type</c> 为 <c>1</c> 时必填。</summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>获取或设置部门 ID（官方 <c>departmentid</c>），<c>type</c> 为 <c>2</c> 时必填。</summary>
    [JsonPropertyName("departmentid")]
    public ulong? Departmentid { get; set; }
}
