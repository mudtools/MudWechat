// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 高级布局宫格用户对象（添加/修改会议高级布局 <c>user_seat_list.user_list</c> 嵌套对象）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class LayoutAdvancedGridUser
{
    /// <summary>获取或设置本场会议企业成员的 userid（userid 与 tmp_openid 必填其一）。</summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>获取或设置用户当前会议临时身份 ID（单场会议唯一；userid 与 tmp_openid 必填其一）。</summary>
    [JsonPropertyName("tmp_openid")]
    public string? TmpOpenid { get; set; }

    /// <summary>
    /// 获取或设置用于视频画面展示的昵称。
    /// <para>添加会议高级布局文档页要求 video_type 为 3（指定人员）时必填且为 base64 编码；获取布局列表文档页同样标注 base64。</para>
    /// </summary>
    [JsonPropertyName("nick_name")]
    public string? NickName { get; set; }
}
