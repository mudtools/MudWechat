// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Store;

/// <summary>
/// 获取门店详情（<c>POST /wxa/get_store_info</c>）响应。
/// </summary>
/// <remarks>
/// <b>官方文档缺陷（照录，SDK 按「字段存在性冲突取并集」超集建模）</b>：官方「返回参数」表
/// <b>仅列 <c>errcode</c>/<c>errmsg</c> 两字段</b>，但「返回示例」实际返回完整的
/// <c>business.base_info</c> 嵌套对象（14 字段 + <c>photo_list</c> 数组），字段表与示例严重不一致；
/// SDK 按示例建模（否则该端点无可消费的响应体）。官方错误码：<c>40001</c>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpStoreInfoResponse : MpResponse
{
    /// <summary>获取或设置门店业务信息（官方 <c>business</c>，仅见返回示例、字段表未收录）。</summary>
    [JsonPropertyName("business")]
    public MpStoreBusiness? Business { get; set; }
}
