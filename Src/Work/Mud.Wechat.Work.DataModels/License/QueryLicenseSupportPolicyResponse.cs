// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.License;

/// <summary>
/// 民生优惠条件查询响应体（<c>/cgi-bin/license/support_policy_query</c>）。
/// </summary>
/// <remarks>
/// <para>官方注记：民生行业接口许可优惠政策于 2023 年 3 月 31 日到期，到期后不再支持查询。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "License")]
public class QueryLicenseSupportPolicyResponse : WechatWorkResponse
{
    /// <summary>获取或设置查询结果：<c>0</c>-不符合减免条件，<c>1</c>-符合减免条件。</summary>
    [JsonPropertyName("query_result")]
    public int? QueryResult { get; set; }

    /// <summary>
    /// 获取或设置被查询企业不符合减免条件的原因对应的错误码列表：
    /// 701090-认证/验证状态不符合 / 701091-行业类型不符合 / 701092-统一社会信用代码不符合 /
    /// 701096-风控审核不通过 / 701110-学校不符合单校条件 / 701111-学校不符合局校条件。
    /// </summary>
    [JsonPropertyName("unsatisfied_reason")]
    public List<int>? UnsatisfiedReason { get; set; }
}
