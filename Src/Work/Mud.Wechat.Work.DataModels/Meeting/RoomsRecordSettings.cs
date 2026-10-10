// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// Rooms 会议室录制配置对象（获取 Rooms 会议室配置项响应 <c>record_settings</c> 嵌套对象）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class RoomsRecordSettings
{
    /// <summary>获取或设置分享云录制：0 - 关闭分享；1 - 全部成员可查看；2 - 仅登录成员可查看；3 - 仅同企业成员可查看；4 - 仅参会成员可查看。</summary>
    [JsonPropertyName("share_record")]
    public int? ShareRecord { get; set; }

    /// <summary>获取或设置是否允许下载云录制：true - 开启；false - 关闭。</summary>
    [JsonPropertyName("download_record")]
    public bool? DownloadRecord { get; set; }
}
