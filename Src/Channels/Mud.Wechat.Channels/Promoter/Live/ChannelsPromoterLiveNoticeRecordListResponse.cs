// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Promoter;

/// <summary>直播预约列表（<c>promoter/get_live_notice_record_list</c>、<c>promoter/get_shop_live_notice_record_list</c>）响应，两端点响应字段集一致 ⇒ 共用。</summary>
[HttpJsonSerializable(SerializerClassName = "Promoter")]
public class ChannelsPromoterLiveNoticeRecordListResponse : ChannelsResponse
{
    /// <summary>获取或设置直播预约数据（官方 <c>live_notice_record_list</c>）。</summary>
    [JsonPropertyName("live_notice_record_list")]
    public List<ChannelsPromoterLiveNoticeRecordInfo>? LiveNoticeRecordList { get; set; }
}
