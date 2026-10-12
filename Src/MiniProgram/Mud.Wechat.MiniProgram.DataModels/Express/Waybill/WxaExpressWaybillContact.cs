// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>面单联系人（<c>contact/get</c> 的发/收件人；地址已合并省市区）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressWaybillContact
{
    /// <summary>用户姓名（<c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>座机号码（<c>tel</c>）。</summary>
    [JsonPropertyName("tel")]
    public string? TelephoneNumber { get; set; }

    /// <summary>手机号码（<c>mobile</c>）。</summary>
    [JsonPropertyName("mobile")]
    public string? MobileNumber { get; set; }

    /// <summary>地址（<c>address</c>，已合并省市区信息）。</summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }
}
