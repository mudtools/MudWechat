// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.DataCube;

/// <summary>
/// 「获取服务号分广告位数据」(<c>publisher_adpos_general</c>) 响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpPublisherAdSlotResponse : MpResponse
{
    /// <summary>回包错误信封（官方 <c>base_resp</c>）。</summary>
    [JsonPropertyName("base_resp")]
    public MpPublisherBaseResp? BaseResp { get; set; }

    /// <summary>分广告位数据列表（官方 <c>list</c>）。</summary>
    [JsonPropertyName("list")]
    public List<MpPublisherAdSlotItem>? DataList { get; set; }

    /// <summary>区间聚合（官方 <c>summary</c>）。</summary>
    [JsonPropertyName("summary")]
    public MpPublisherAdSlotSummary? Summary { get; set; }

    /// <summary>list 返回总条数（官方 <c>total_num</c>）。</summary>
    [JsonPropertyName("total_num")]
    public long TotalNum { get; set; }
}
