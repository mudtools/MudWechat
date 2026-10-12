// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>消息入口按钮（官方 <c>*_msg_operation.url_cell</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardUrlCell
{
    /// <summary>获取或设置生效的卡券模板列表（官方 <c>card_id_list</c>，空表示对全部卡生效）。</summary>
    [JsonPropertyName("card_id_list")]
    public List<string>? CardIdList { get; set; }

    /// <summary>获取或设置按钮失效时间戳（官方 <c>end_time</c>，秒）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>获取或设置按钮文字（官方 <c>text</c>）。</summary>
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    /// <summary>获取或设置按钮跳转外链（官方 <c>url</c>）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>获取或设置按钮跳转小程序原始 id（官方 <c>app_brand_id</c>）。</summary>
    [JsonPropertyName("app_brand_id")]
    public string? AppBrandId { get; set; }

    /// <summary>获取或设置按钮跳转小程序页面路径（官方 <c>app_brand_pass</c>）。</summary>
    [JsonPropertyName("app_brand_pass")]
    public string? AppBrandPass { get; set; }
}
