// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Hr;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「人事助手」模块花名册域企业自建应用 SDK（获取员工字段配置 / 获取员工花名册信息 / 更新员工花名册信息）。
/// <para>
/// 官方仅向自建应用开放本域 3 个端点（代开发应用与第三方应用均暂不支持），
/// 声明于本接口（形态对齐 <see cref="IWechatWorkInternalMsgAuditPermitUserService"/>）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>）。
/// 官方权限口径：调用应用需配置到「人事助手 - 可调用接口的应用」中。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Hr",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkHrService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.InternalAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalHrService : IWechatWorkHrService
{
    /// <summary>
    /// 获取员工字段配置
    /// <para>获取人事助手花名册的字段组配置（group_list：字段组 id / 名称 / 字段列表——
    /// 字段 id / 名称 / 类型 / 值类型 / 是否必填 / 选项枚举），用于组装后续花名册读取与更新的 fieldids。</para>
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>字段组配置信息列表（group_list：group_id / group_name / field_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99131"/></para>
    /// <para>官方业务限制：代开发应用、第三方应用均暂不支持本接口。</para>
    /// <para>官方文档陷阱：字段信息说明表将字段 id 记作 field_id，请求/响应示例均为 fieldid，本仓以示例为准。</para>
    /// </remarks>
    [Get("/cgi-bin/hr/get_fields")]
    Task<GetHrFieldsResponse> GetFieldsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取员工花名册信息
    /// <para>获取员工的花名册字段信息（get_all 拉取全部，或按 fieldids 指定字段拉取）；
    /// 各字段以 fieldid + sub_idx 定位，查询结果 result（1 成功 / 2 失败 / 3 字段未找到 / 5 不支持获取的字段类型）。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetHrStaffInfoRequest"/>：userid / get_all / fieldids）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>字段信息列表（field_info：fieldid / sub_idx / result / value_type / value_xxxxx）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99132"/></para>
    /// <para>官方业务限制：代开发应用、第三方应用均暂不支持本接口。</para>
    /// </remarks>
    [Post("/cgi-bin/hr/get_staff_info")]
    Task<GetHrStaffInfoResponse> GetStaffInfoAsync(
        [Body] GetHrStaffInfoRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新员工花名册信息
    /// <para>更新、增加或清空员工的花名册字段信息：update_items 更新单个字段（可清空）、
    /// remove_items 整组删除可重复字段组、insert_items 增加一组可重复字段组——三者不能全部为空。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="UpdateHrStaffInfoRequest"/>：userid / update_items / remove_items / insert_items）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99133"/></para>
    /// <para>官方业务限制：代开发应用、第三方应用均暂不支持本接口；
    /// 目标员工须在调用应用的可见范围内；官方标注不支持更新的字段：11006（年龄）、11012（社会工龄）、12004（员工状态）。</para>
    /// </remarks>
    [Post("/cgi-bin/hr/update_staff_info")]
    Task<WechatWorkResponse> UpdateStaffInfoAsync(
        [Body] UpdateHrStaffInfoRequest request,
        CancellationToken cancellationToken = default);
}
