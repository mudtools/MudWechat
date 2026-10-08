// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.School;

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「家校沟通」模块部门管理域公共 SDK。
/// <para>
/// 官方对三类应用开放完全一致的 5 个端点（部门增删改查 + 自动升年级配置），
/// 全部收敛声明于本接口；应用类型子接口均为零差异端点空标记：
/// 自建见 <see cref="IWechatWorkInternalSchoolDepartmentService"/>，
/// 第三方见 <see cref="IWechatWorkThirdPartySchoolDepartmentService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderSchoolDepartmentService"/>。
/// </para>
/// <para>
/// 家校沟通域其它功能族：<see cref="IWechatWorkSchoolService"/>（基础域）、
/// <see cref="IWechatWorkSchoolSettingService"/>（家校管理配置域）、
/// <see cref="IWechatWorkSchoolUserService"/>（学生与家长管理域）、
/// <see cref="IWechatWorkSchoolAuthService"/>（网页授权登录域）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>）：
/// 企业自建为应用自身 access_token；第三方 / 代开发为授权企业级 access_token（scope = authCorpId），
/// 由多应用基座按当前应用上下文路由——企业级令牌须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// </para>
/// <para>
/// 权限口径：写入族端点（创建/更新/删除部门、修改自动升年级配置）——自建应用须配置到
/// 「家校沟通-读取和编辑家校通讯录的应用」，第三方应用须具有「家校沟通」使用和编辑权限，
/// 代开发应用须拥有「家校沟通」可使用家校通讯录-家校通讯录编辑权限；
/// 读取族端点（获取部门列表）——自建应用亦接受「家校沟通-读取家校通讯录的应用」或
/// 「家长可使用应用」列表，第三方应用具有「家校沟通」使用权限，代开发应用具有「家校沟通」权限。
/// 自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口（存量企业暂不受影响）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkSchoolDepartmentService
{
    /// <summary>
    /// 创建部门
    /// <para>在家校通讯录中创建部门（班级 / 年级 / 学段 / 校区）。</para>
    /// <para>官方业务限制：班级的父部门必须是年级（type 为 1 的部门的父部门 type 值必须为 2）；
    /// 管理员类型须与部门类型保持一致（校区负责人只能配置到校区部门，班主任和任课老师只能设置到班级）；
    /// 当设置了入学年份和标准年级时，name 参数将被忽略；
    /// 指定部门 id 时必须大于 1，若不填将自动生成 id。</para>
    /// </summary>
    /// <param name="request">创建部门请求体（<see cref="SchoolCreateDepartmentRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>创建结果（id 为创建的部门 id）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92340"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92296"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100158"/></para>
    /// </remarks>
    [Post("/cgi-bin/school/department/create")]
    Task<SchoolCreateDepartmentResponse> CreateDepartmentAsync(
        [Body] SchoolCreateDepartmentRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新部门
    /// <para>更新家校通讯录中的部门信息（含管理员增删改与新 id 变更）。</para>
    /// <para>官方业务限制：班级的父部门必须是年级（type 为 1 的部门的父部门 type 值必须为 2）；
    /// 当传入的 standard_grade 为 0 时，表示将此部门转换为非标准年级；
    /// 管理员类型须与部门类型保持一致；
    /// 部门管理员列表为逐条操作语义（op=0 新增或更新，op=1 删除）。</para>
    /// </summary>
    /// <param name="request">更新部门请求体（<see cref="SchoolUpdateDepartmentRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>更新结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92341"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92297"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100159"/></para>
    /// </remarks>
    [Post("/cgi-bin/school/department/update")]
    Task<WechatWorkResponse> UpdateDepartmentAsync(
        [Body] SchoolUpdateDepartmentRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除部门（官方即 GET，id 走 Query）
    /// <para>从家校通讯录中删除部门。</para>
    /// <para>官方业务限制：不能删除根部门；不能删除含有子部门、成员的部门。</para>
    /// </summary>
    /// <param name="departmentId">部门 id（官方参数表将 id 标注为「否」，照抄不纠正；
    /// 语义上删除操作必须传入目标部门 id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>删除结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92342"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92298"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100160"/></para>
    /// </remarks>
    [Get("/cgi-bin/school/department/delete")]
    Task<WechatWorkResponse> DeleteDepartmentAsync(
        [Query("id")] int departmentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取部门列表（官方即 GET，id 走 Query）
    /// <para>获取家校通讯录的部门列表。</para>
    /// <para>官方业务限制：不填 id 时默认获取全量组织架构，填 id 时获取指定部门及其下的子部门；
    /// 响应中部分字段为条件返回（入学年份/标准年级仅标准年级返回、是否已毕业仅年级返回、
    /// 班级群字段仅班级类型返回、order 仅在 API 设置后才返回）。</para>
    /// </summary>
    /// <param name="departmentId">部门 id（可选：获取指定部门及其下的子部门；
    /// 不填默认获取全量组织架构）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>部门列表（departments，根部门 id 固定为 1、type 为 5 学校）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92343"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92299"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96745"/></para>
    /// </remarks>
    [Get("/cgi-bin/school/department/list")]
    Task<SchoolDepartmentListResponse> ListDepartmentsAsync(
        [Query("id")] int? departmentId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改自动升年级的配置
    /// <para>设置自动升年级的开关与时间。</para>
    /// <para>官方业务限制：upgrade_time 时间戳只有月和日有效，不传则默认为传 0（代表 1 月 1 号）；
    /// upgrade_switch 不传默认关闭，官方语义「传所有非 1 的值也视为关闭」。</para>
    /// </summary>
    /// <param name="request">设置请求体（<see cref="SchoolSetUpgradeInfoRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>设置结果（next_upgrade_time 为下次升级的时间，只有年月日有效）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92949"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92950"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100161"/></para>
    /// </remarks>
    [Post("/cgi-bin/school/set_upgrade_info")]
    Task<SchoolSetUpgradeInfoResponse> SetUpgradeInfoAsync(
        [Body] SchoolSetUpgradeInfoRequest request,
        CancellationToken cancellationToken = default);
}
