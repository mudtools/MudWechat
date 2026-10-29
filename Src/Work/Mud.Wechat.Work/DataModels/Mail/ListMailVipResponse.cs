// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Mail;

/// <summary>
/// 获取高级功能账号列表响应体（<c>/cgi-bin/exmail/vip/list</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Mail")]
public class ListMailVipResponse : WechatWorkResponse
{
    /// <summary>获取或设置是否还有更多数据未获取（官方 has_more）。</summary>
    /// <remarks>
    /// <para>
    /// 官方响应示例作布尔值（<c>true</c>）；官方同时注明：不保证每次返回的数据刚好为指定 limit，
    /// 必须用返回的 has_more 判断是否继续请求。
    /// </para>
    /// </remarks>
    [JsonPropertyName("has_more")]
    public bool? HasMore { get; set; }

    /// <summary>获取或设置下一次请求的 cursor 值（官方 next_cursor）。</summary>
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; set; }

    /// <summary>获取或设置符合条件的企业成员 userid 列表（官方 userid_list）。</summary>
    [JsonPropertyName("userid_list")]
    public List<string>? UseridList { get; set; }
}
