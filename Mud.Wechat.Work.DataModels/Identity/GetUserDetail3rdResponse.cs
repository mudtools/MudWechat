// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Identity;

/// <summary>
/// 获取访问用户敏感信息响应体（第三方，<c>/cgi-bin/service/auth/getuserdetail3rd</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约陷阱：① 与自建/代开发版（<see cref="GetUserDetailResponse"/>）响应面不同——
/// 本响应多 <see cref="Corpid"/> 与 <see cref="Name"/>，<b>不含</b> mobile / email / biz_mail / address；
/// ② <see cref="Name"/> 自 2019-12-30 起对新创建第三方应用不再返回真实姓名（以 userid 代替），
/// 2020-06-30 起覆盖所有历史第三方应用；③ <see cref="Gender"/> 为<b>字符串</b>枚举（官方示例即 <c>"1"</c>）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Identity")]
public class GetUserDetail3rdResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置用户所属企业的 corpid。
    /// </summary>
    [JsonPropertyName("corpid")]
    public string? Corpid { get; set; }

    /// <summary>
    /// 获取或设置成员 UserID。
    /// </summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>
    /// 获取或设置成员姓名（自 2019-12-30 起不再返回真实姓名，以 userid 代替返回；展示姓名须用通讯录展示组件）。
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 获取或设置性别（字符串："0" 未定义 / "1" 男 / "2" 女）；
    /// 仅在用户同意 snsapi_privateinfo 授权时返回真实值，否则返回 "0"。
    /// </summary>
    [JsonPropertyName("gender")]
    public string? Gender { get; set; }

    /// <summary>
    /// 获取或设置头像 url；仅在用户同意 snsapi_privateinfo 授权时返回真实头像，否则返回默认头像。
    /// </summary>
    [JsonPropertyName("avatar")]
    public string? Avatar { get; set; }

    /// <summary>
    /// 获取或设置员工个人二维码（扫描可添加为外部联系人）；仅在用户同意 snsapi_privateinfo 授权时返回。
    /// </summary>
    [JsonPropertyName("qr_code")]
    public string? QrCode { get; set; }
}
