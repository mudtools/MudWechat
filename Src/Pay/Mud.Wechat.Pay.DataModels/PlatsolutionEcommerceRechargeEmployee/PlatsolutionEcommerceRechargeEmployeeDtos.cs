// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceRechargeEmployee;

/// <summary>添加二级商户可扫码充值员工请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013521743"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceRechargeEmployee")]
public class RechargeEmployeeAddRequest
{
    /// <summary>【二级商户号】 二级商户号（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【员工OpenID】 二级商户的员工在平台商户应用下的OpenID（官方字段路径 <c>sp_openid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_openid")]
    public string? SpOpenid { get; set; }

}

/// <summary>添加二级商户可扫码充值员工响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013521743"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceRechargeEmployee")]
public class RechargeEmployeeAddResponse : WechatPayResponse
{
    /// <summary>【二级商户号】 二级商户号（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【员工OpenID】 二级商户的员工在平台商户应用下的OpenID（官方字段路径 <c>sp_openid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_openid")]
    public string? SpOpenid { get; set; }

    /// <summary>【员工状态】 员工状态。可选取值：`EFFECTIVE` 生效中 / `DELETED` 已删除（官方字段路径 <c>state</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>【添加员工时间】 添加员工时间，使用rfc3339标准格式（官方字段路径 <c>create_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>【最后更新时间】 最后更新时间，使用rfc3339标准格式（官方字段路径 <c>update_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }

}

/// <summary>查询二级商户可扫码充值员工列表响应·【员工列表】 包含当前所有的可充值员工。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013521757"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceRechargeEmployee")]
public class RechargeEmployeeListResponseEmployeesItem
{
    /// <summary>【二级商户号】 二级商户号（官方字段路径 <c>employees.sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【员工OpenID】 二级商户的员工在平台商户应用下的OpenID（官方字段路径 <c>employees.sp_openid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_openid")]
    public string? SpOpenid { get; set; }

    /// <summary>【员工状态】 员工状态。可选取值：`EFFECTIVE` 生效中 / `DELETED` 已删除（官方字段路径 <c>employees.state</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>【添加员工时间】 添加员工时间，使用rfc3339标准格式（官方字段路径 <c>employees.create_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>【最后更新时间】 最后更新时间，使用rfc3339标准格式（官方字段路径 <c>employees.update_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }

}

/// <summary>查询二级商户可扫码充值员工列表响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013521757"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceRechargeEmployee")]
public class RechargeEmployeeListResponse : WechatPayResponse
{
    /// <summary>【员工列表】 包含当前所有的可充值员工（官方字段路径 <c>employees</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("employees")]
    public List<RechargeEmployeeListResponseEmployeesItem>? Employees { get; set; }

}
