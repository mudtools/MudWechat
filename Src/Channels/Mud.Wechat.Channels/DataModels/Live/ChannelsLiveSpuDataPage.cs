// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Live;

/// <summary>电商商品数据页（官方 <c>single_live_ec_spu_data_page_v2</c> 对象）。</summary>
/// <remarks><b>官方文档矛盾照录</b>：参数表把 <c>spu_data_list</c> 类型标注为 object（名称含 list）——按参数表（单对象）建模。</remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLiveSpuDataPage
{
    /// <summary>获取或设置商品明细数据（官方 <c>spu_data_list</c>，参数表标注为 object）。</summary>
    [JsonPropertyName("spu_data_list")]
    public ChannelsLiveSpuData? SpuDataList { get; set; }

    /// <summary>获取或设置 <c>spu_data_list</c> 的总长度（官方 <c>total_cnt</c>）。</summary>
    [JsonPropertyName("total_cnt")]
    public long? TotalCnt { get; set; }
}
