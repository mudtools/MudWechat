// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.ShopAccount;

/// <summary>
/// 完成接入任务请求（<c>POST /shop/register/finish_access_info</c>）。
/// </summary>
/// <remarks>
/// <para><b>本端点是「向微信申报某项接入任务已完成」的单向申报</b>：官方请求体只有一支枚举，
/// 完成状态本身由 <c>/shop/register/check</c> 的 <c>access_info</c> 回读（两个方向、两套字段名）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAccount")]
public class WxaShopFinishAccessRequest
{
    /// <summary>
    /// 要标记完成的接入任务项（<c>access_info_item</c>，number，<b>必填</b>）。
    /// 官方取值：<c>6</c> 完成 spu 接口 / <c>7</c> 完成订单接口 / <c>19</c> 完成二级商户号订单 /
    /// <c>8</c> 完成物流接口 / <c>9</c> 完成售后接口 / <c>20</c> 完成二级商户号售后 /
    /// <c>10</c> 测试完成 / <c>11</c> 发版完成。
    /// <b>注意官方编号不连续</b>（19、20 夹在 7~8 与 9~10 之间的语义位是二级商户号分支）⇒ 不得按连续序号生成。
    /// </summary>
    [JsonPropertyName("access_info_item")]
    public long? AccessInfoItem { get; set; }
}
