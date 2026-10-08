// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.School;

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「家校沟通」模块家校管理配置域公共 SDK
/// （老师可查看班级模式 + 手机号转外部联系人 ID）。
/// <para>
/// 官方<b>仅向企业自建应用与第三方应用</b>开放本域端点（官方权限口径未列代开发应用，
/// 代开发文档树亦无对应文档，不设代开发子接口）；
/// 全部 3 个端点收敛声明于本接口：自建见 <see cref="IWechatWorkInternalSchoolSettingService"/>，
/// 第三方见 <see cref="IWechatWorkThirdPartySchoolSettingService"/>
/// （形态对齐异步导入域 <see cref="IWechatWorkBatchService"/>：仅自建 + 第三方子接口）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 企业自建消费应用自身 access_token；第三方消费授权企业级 access_token（scope = authCorpId）。
/// 令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkSchoolSettingService
{
    /// <summary>
    /// 设置「老师可查看班级」的模式
    /// <para>配置教师查看班级为「全部班级」或「仅负责范围内的班级」。</para>
    /// </summary>
    /// <param name="request">设置请求体（<see cref="SetSchoolTeacherViewModeRequest"/>：
    /// view_mode（1 全部班级 / 2 仅负责范围内的班级））。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>设置结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92652"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92652"/></para>
    /// <para>官方业务限制：服务商代开发官方未开放本接口。</para>
    /// </remarks>
    [Post("/cgi-bin/school/set_teacher_view_mode")]
    Task<WechatWorkResponse> SetTeacherViewModeAsync(
        [Body] SetSchoolTeacherViewModeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取「老师可查看班级」的模式
    /// <para>获取教师查看班级的当前模式（全部班级或仅负责范围内的班级）。</para>
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>查看模式（view_mode：1 全部班级 / 2 仅负责范围内的班级）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92652"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92652"/></para>
    /// <para>官方业务限制：服务商代开发官方未开放本接口。</para>
    /// </remarks>
    [Get("/cgi-bin/school/get_teacher_view_mode")]
    Task<GetSchoolTeacherViewModeResponse> GetTeacherViewModeAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 手机号转外部联系人 ID
    /// <para>批量将家长手机号转换为外部联系人 userid。</para>
    /// <para>官方业务限制：一次最多支持 100 个手机号；
    /// external_userid 仅在家长关注后才会返回。</para>
    /// </summary>
    /// <param name="request">转换请求体（<see cref="SchoolBatchToExternalUserIdRequest"/>：
    /// mobiles（家长手机号列表，最多 100 个））。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>转换结果（success_list 成功列表 / fail_list 失败列表）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92506"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92506"/></para>
    /// <para>官方业务限制：服务商代开发官方未开放本接口。</para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/batch_to_external_userid")]
    Task<SchoolBatchToExternalUserIdResponse> BatchToExternalUserIdAsync(
        [Body] SchoolBatchToExternalUserIdRequest request,
        CancellationToken cancellationToken = default);
}
