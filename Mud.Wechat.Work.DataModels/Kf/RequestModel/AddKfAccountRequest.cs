// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Kf;

/// <summary>
/// 添加客服账号请求体（<c>/cgi-bin/kf/account/add</c>）。
/// <para>
/// <see cref="Name"/> 与 <see cref="MediaId"/> 均为官方必填；
/// 一家企业最多可添加 5000 个客服账号。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class AddKfAccountRequest
{
    /// <summary>
    /// 获取或设置客服名称（官方必填，不多于 16 个字符）。
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 获取或设置客服头像临时素材 media_id（官方必填，可调用上传临时素材接口获取，不多于 128 个字节）。
    /// </summary>
    [JsonPropertyName("media_id")]
    public string? MediaId { get; set; }
}
