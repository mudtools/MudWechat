// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 会议高级布局座次对象（添加/修改会议高级布局与获取布局列表/用户布局响应 <c>user_seat_list</c> 嵌套对象）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档形态差异：添加/修改会议高级布局文档页与获取会议布局列表 / 获取用户布局文档页的座次参数表仅按<b>高级形态</b>描述
/// （<c>grid_id</c> / <c>grid_type</c> / <c>video_type</c> / <c>user_list</c>），而「获取会议布局列表」文档页的功能描述为返回会议
/// 「基础和高级自定义布局信息列表」、响应中又无 <c>layout_type</c> 之类区分字段，故基础布局经本端点返回时的座次形态官方未作定义。
/// </para>
/// <para>
/// 本模型同时承载高级形态字段（<see cref="VideoType"/> / <see cref="UserList"/>）与基础布局形态的扁平字段
/// （<see cref="FlatUserid"/> / <see cref="FlatTmpOpenid"/> / <see cref="FlatNickName"/> / <see cref="FlatToolSdkid"/>，
/// 即基础布局请求中位于座次对象上的字段），以保证两种形态均可反序列化；未赋值的字段不参与序列化。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class LayoutAdvancedSeat
{
    /// <summary>获取或设置宫格 ID（官方必填）。</summary>
    [JsonPropertyName("grid_id")]
    public string? GridId { get; set; }

    /// <summary>获取或设置宫格类型（官方必填）：1 - 视频画面；2 - 共享画面。</summary>
    [JsonPropertyName("grid_type")]
    public int? GridType { get; set; }

    /// <summary>获取或设置视频画面来源：1 - 演讲者；2 - 自动填充；3 - 指定人员（此类型需传递 tmp_openid）。</summary>
    [JsonPropertyName("video_type")]
    public int? VideoType { get; set; }

    /// <summary>
    /// 获取或设置宫格中的用户列表（详见 <see cref="LayoutAdvancedGridUser"/>）。
    /// <para>轮询关闭时只有一个用户；轮询开启后可以包含多个用户。</para>
    /// </summary>
    [JsonPropertyName("user_list")]
    public List<LayoutAdvancedGridUser>? UserList { get; set; }

    /// <summary>获取或设置当场会议的企业成员 userid（基础布局经获取布局列表端点返回时的扁平形态字段）。</summary>
    [JsonPropertyName("userid")]
    public string? FlatUserid { get; set; }

    /// <summary>获取或设置当场会议的用户临时 ID（基础布局经获取布局列表端点返回时的扁平形态字段）。</summary>
    [JsonPropertyName("tmp_openid")]
    public string? FlatTmpOpenid { get; set; }

    /// <summary>获取或设置昵称（基础布局经获取布局列表端点返回时的扁平形态字段，作为视频画面展示）。</summary>
    [JsonPropertyName("nick_name")]
    public string? FlatNickName { get; set; }

    /// <summary>获取或设置拓展应用 ID（基础布局经获取布局列表端点返回时的扁平形态字段）。</summary>
    [JsonPropertyName("tool_sdkid")]
    public string? FlatToolSdkid { get; set; }
}
