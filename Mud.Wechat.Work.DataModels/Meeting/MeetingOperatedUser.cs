// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 会控被操作成员对象（管理联席主持人 / 静音成员 / 关闭成员屏幕共享 / 关闭或开启成员视频请求
/// <c>operated_user</c>，以及管理等候室成员 / 移出成员请求 <c>operated_users</c> 元素）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档陷阱：管理联席主持人 / 静音成员 / 关闭成员屏幕共享 / 关闭或开启成员视频四端点的参数表将
/// <c>operated_user</c> 标注为 object[]，但官方请求示例与字段命名单复数口径均为单个对象
/// （复数命名的 <c>operated_users</c> 承载数组），故本模型以单个对象承载。
/// 关闭或开启成员视频文档页 User 参数表将设备类型字段误写为 <c>instanceid</c>，官方示例及其余会控端点均为
/// <c>instance_id</c>，本模型以 <c>instance_id</c> 为准。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class MeetingOperatedUser
{
    /// <summary>获取或设置被操作者的会中临时 ID（官方必填；可通过获取实时会中成员列表等接口获得）。</summary>
    [JsonPropertyName("tmp_openid")]
    public string? TmpOpenid { get; set; }

    /// <summary>
    /// 获取或设置成员的终端设备类型（官方必填；请与被操作者的设备类型保持一致，否则不生效）。
    /// <para>取值：0 - PSTN；1 - PC；2 - Mac；3 - Android；4 - iOS；5 - Web；6 - iPad；7 - Android Pad；
    /// 8 - 小程序；9 - voip、sip 设备；10 - linux；20 - Rooms for Touch Windows；21 - Rooms for Touch MacOS；
    /// 22 - Rooms for Touch Android；30 - Controller for Touch Windows；32 - Controller for Touch Android；
    /// 33 - Controller for Touch iOS；81 - 鸿蒙手机；82 - 鸿蒙平板；83 - 鸿蒙 PC；84 - 鸿蒙汽车。
    /// 各端点支持的设备类型集合不同（如管理联席主持人不支持 PSTN/小程序等），以各端点官方文档为准。</para>
    /// </summary>
    [JsonPropertyName("instance_id")]
    public int? InstanceId { get; set; }
}
