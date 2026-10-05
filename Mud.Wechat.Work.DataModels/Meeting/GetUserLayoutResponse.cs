// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 获取用户布局响应体（<c>/cgi-bin/meeting/advanced_layout/get_user_layout</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class GetUserLayoutResponse : WechatWorkResponse
{
    /// <summary>获取或设置会议应用的布局 ID。</summary>
    [JsonPropertyName("selected_layout_id")]
    public string? SelectedLayoutId { get; set; }

    /// <summary>获取或设置布局名称。</summary>
    [JsonPropertyName("layout_name")]
    public string? LayoutName { get; set; }

    /// <summary>获取或设置布局类型：0 - 默认布局；2 - 自定义会议布局；3 - 个性布局。</summary>
    [JsonPropertyName("layout_type")]
    public int? LayoutType { get; set; }

    /// <summary>获取或设置布局单页对象列表（详见 <see cref="LayoutAdvancedPage"/>）。</summary>
    [JsonPropertyName("page_list")]
    public List<LayoutAdvancedPage>? PageList { get; set; }
}
