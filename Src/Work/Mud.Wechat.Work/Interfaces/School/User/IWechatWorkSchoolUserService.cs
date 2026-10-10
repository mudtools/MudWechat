// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.School;

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「家校沟通」模块学生与家长管理域公共 SDK。
/// <para>
/// 官方对三类应用开放完全一致的 16 个端点（学生/家长的增删改、批量增删改、读取、
/// 部门学生/家长详情、家校通讯录自动同步模式），全部收敛声明于本接口；
/// 应用类型子接口均为零差异端点空标记：自建见 <see cref="IWechatWorkInternalSchoolUserService"/>，
/// 第三方见 <see cref="IWechatWorkThirdPartySchoolUserService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderSchoolUserService"/>。
/// </para>
/// <para>
/// 家校沟通域其它功能族：<see cref="IWechatWorkSchoolService"/>（基础域）、
/// <see cref="IWechatWorkSchoolSettingService"/>（家校管理配置域）、
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
/// 权限口径：自建应用须配置到「家校沟通-读取和编辑家校通讯录的应用」（读取族亦接受
/// 「家校沟通-读取家校通讯录的应用」或「家长可使用应用」）；第三方应用须具有「家校沟通」使用（读取族）/使用和编辑（写入族）权限；
/// 代开发应用须具有「家校沟通」可使用家校通讯录-家校通讯录编辑（写入族）/「家校沟通」（读取族）权限。
/// 自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口（存量企业暂不受影响）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkSchoolUserService
{
    /// <summary>
    /// 创建学生
    /// <para>在家校通讯录中创建单个学生。</para>
    /// <para>官方业务限制：参数字段超过长度限制时整个请求会被拦掉；
    /// department 班级 id 列表不超过 20 个；to_invite 仅验证的学校才能发起邀请。</para>
    /// </summary>
    /// <param name="request">创建学生请求体（<see cref="SchoolCreateStudentRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>创建结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92325"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92035"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100145"/></para>
    /// </remarks>
    [Post("/cgi-bin/school/user/create_student")]
    Task<WechatWorkResponse> CreateStudentAsync(
        [Body] SchoolCreateStudentRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除学生（官方即 GET，userid 走 Query）
    /// <para>从家校通讯录中删除单个学生。</para>
    /// <para>官方业务限制：如果学生还有家长，则不允许删除学生，必须先把家长删除。</para>
    /// </summary>
    /// <param name="userId">家校通讯录中学生的 userid。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>删除结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92326"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92039"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100146"/></para>
    /// </remarks>
    [Get("/cgi-bin/school/user/delete_student")]
    Task<WechatWorkResponse> DeleteStudentAsync(
        [Query("userid")] string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新学生
    /// <para>更新家校通讯录中单个学生的信息。</para>
    /// <para>官方业务限制：参数字段超过长度限制时整个请求会被拦掉；
    /// 每个班级下学生总数不能超过 3 万个，建议保证创建 department 对应的部门和创建成员是串行化处理；
    /// new_student_userid 每个学生仅能修改一次。</para>
    /// </summary>
    /// <param name="request">更新学生请求体（<see cref="SchoolUpdateStudentRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>更新结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92327"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92041"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100147"/></para>
    /// </remarks>
    [Post("/cgi-bin/school/user/update_student")]
    Task<WechatWorkResponse> UpdateStudentAsync(
        [Body] SchoolUpdateStudentRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量创建学生
    /// <para>在家校通讯录中批量创建学生（部分失败通过 result_list 逐条返回）。</para>
    /// <para>官方业务限制：每次最多 100 个学生；每个班级下学生总数不能超过 3 万个，
    /// 建议保证创建 department 对应的部门和创建成员是串行化处理。</para>
    /// </summary>
    /// <param name="request">批量创建学生请求体（<see cref="SchoolBatchCreateStudentRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>批量结果（result_list 失败的学生列表）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92328"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92037"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100148"/></para>
    /// </remarks>
    [Post("/cgi-bin/school/user/batch_create_student")]
    Task<SchoolBatchStudentResultResponse> BatchCreateStudentAsync(
        [Body] SchoolBatchCreateStudentRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量删除学生（官方批量删除即 POST，useridlist 请求体）
    /// <para>从家校通讯录中批量删除学生（部分失败通过 result_list 逐条返回）。</para>
    /// <para>官方业务限制：每次最多 100 个；
    /// 如果学生还有家长，则不允许删除学生，必须先把家长删除。</para>
    /// </summary>
    /// <param name="request">批量删除学生请求体（<see cref="SchoolBatchDeleteStudentRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>批量结果（result_list 逐条删除结果）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92329"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92040"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100149"/></para>
    /// </remarks>
    [Post("/cgi-bin/school/user/batch_delete_student")]
    Task<SchoolBatchStudentResultResponse> BatchDeleteStudentAsync(
        [Body] SchoolBatchDeleteStudentRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量更新学生
    /// <para>批量更新家校通讯录中的学生信息（部分失败通过 result_list 逐条返回）。</para>
    /// <para>官方业务限制：每次最多 100 个学生；
    /// 每个班级下学生总数不能超过 3 万个，建议保证创建 department 对应的部门和创建成员是串行化处理；
    /// new_student_userid 每个学生仅能修改一次。</para>
    /// </summary>
    /// <param name="request">批量更新学生请求体（<see cref="SchoolBatchUpdateStudentRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>批量结果（result_list 失败的学生列表）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92330"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92042"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100150"/></para>
    /// </remarks>
    [Post("/cgi-bin/school/user/batch_update_student")]
    Task<SchoolBatchStudentResultResponse> BatchUpdateStudentAsync(
        [Body] SchoolBatchUpdateStudentRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建家长
    /// <para>在家校通讯录中创建单个家长并建立与学生（孩子）的关系。</para>
    /// <para>官方业务限制：children 孩子列表最多 10 个；
    /// to_invite 仅验证的学校才能发起邀请；参数字段超过长度限制时整个请求会被拦掉。</para>
    /// </summary>
    /// <param name="request">创建家长请求体（<see cref="SchoolCreateParentRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>创建结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92331"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92077"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100151"/></para>
    /// </remarks>
    [Post("/cgi-bin/school/user/create_parent")]
    Task<WechatWorkResponse> CreateParentAsync(
        [Body] SchoolCreateParentRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除家长（官方即 GET，userid 走 Query）
    /// <para>从家校通讯录中删除单个家长。</para>
    /// </summary>
    /// <param name="userId">家校通讯录中家长的 userid。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>删除结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92332"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92079"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100152"/></para>
    /// </remarks>
    [Get("/cgi-bin/school/user/delete_parent")]
    Task<WechatWorkResponse> DeleteParentAsync(
        [Query("userid")] string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新家长
    /// <para>更新家校通讯录中单个家长的信息。</para>
    /// <para>官方业务限制：children 为<b>全量更新</b>——官方原文「该字段是全量更新，
    /// 如果孩子列表为空则忽略该字段」，即传非空列表为全量覆盖、传空数组不是删除全部孩子关系；
    /// new_parent_userid 每个家长仅能更新一次。</para>
    /// </summary>
    /// <param name="request">更新家长请求体（<see cref="SchoolUpdateParentRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>更新结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92333"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92081"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100153"/></para>
    /// </remarks>
    [Post("/cgi-bin/school/user/update_parent")]
    Task<WechatWorkResponse> UpdateParentAsync(
        [Body] SchoolUpdateParentRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量创建家长
    /// <para>在家校通讯录中批量创建家长（部分失败通过 result_list 逐条返回）。</para>
    /// <para>官方业务限制：每次最多 100 个家长；每个家长的孩子列表最多 10 个。</para>
    /// </summary>
    /// <param name="request">批量创建家长请求体（<see cref="SchoolBatchCreateParentRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>批量结果（result_list 失败的家长列表）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92334"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92078"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100154"/></para>
    /// </remarks>
    [Post("/cgi-bin/school/user/batch_create_parent")]
    Task<SchoolBatchParentResultResponse> BatchCreateParentAsync(
        [Body] SchoolBatchCreateParentRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量删除家长（官方批量删除即 POST，useridlist 请求体）
    /// <para>从家校通讯录中批量删除家长（部分失败通过 result_list 逐条返回）。</para>
    /// <para>官方业务限制：每次最多 100 个。</para>
    /// </summary>
    /// <param name="request">批量删除家长请求体（<see cref="SchoolBatchDeleteParentRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>批量结果（result_list 逐条删除结果）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92335"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92080"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100155"/></para>
    /// </remarks>
    [Post("/cgi-bin/school/user/batch_delete_parent")]
    Task<SchoolBatchParentResultResponse> BatchDeleteParentAsync(
        [Body] SchoolBatchDeleteParentRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量更新家长
    /// <para>批量更新家校通讯录中的家长信息（部分失败通过 result_list 逐条返回）。</para>
    /// <para>官方业务限制：每次最多 100 个家长；每个家长的孩子列表最多 10 个；
    /// new_parent_userid 每个家长仅能更新一次；每个班级下学生总数不能超过 3 万个。</para>
    /// </summary>
    /// <param name="request">批量更新家长请求体（<see cref="SchoolBatchUpdateParentRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>批量结果（result_list 失败的家长列表）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92336"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92082"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100156"/></para>
    /// </remarks>
    [Post("/cgi-bin/school/user/batch_update_parent")]
    Task<SchoolBatchParentResultResponse> BatchUpdateParentAsync(
        [Body] SchoolBatchUpdateParentRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 读取学生或家长（官方即 GET，userid 走 Query）
    /// <para>按 userid 读取家校通讯录中的学生或家长详情。</para>
    /// <para>官方业务限制：家长手机号第三方应用不可获取，代开发应用需管理员授权手机号权限才返回；
    /// external_userid 仅当家长已关注「学校通知」才返回。</para>
    /// </summary>
    /// <param name="userId">家校通讯录的 userid（家长或者学生的 userid，
    /// 不区分大小写，长度为 1~64 个字节）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>学生或家长详情（user_type：1 学生返回 student / 2 家长返回 parent）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92337"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92038"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96738"/></para>
    /// </remarks>
    [Get("/cgi-bin/school/user/get")]
    Task<GetSchoolUserResponse> GetSchoolUserAsync(
        [Query("userid")] string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取部门学生详情（官方即 GET，department_id 走 Query）
    /// <para>获取指定班级（部门）下的学生详情列表。</para>
    /// <para>官方业务限制：如需获取该部门及其子部门的所有学生，需先获取该部门下的子部门，
    /// 然后再获取子部门下的学生，逐层递归获取；官方无 cursor/limit 分页参数；
    /// 家长手机号第三方应用不可获取。</para>
    /// </summary>
    /// <param name="departmentId">获取的部门 id（班级）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>学生详情列表（students，含每个学生的家长列表）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92338"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92043"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96739"/></para>
    /// </remarks>
    [Get("/cgi-bin/school/user/list")]
    Task<SchoolDepartmentStudentsResponse> ListDepartmentStudentsAsync(
        [Query("department_id")] int departmentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取部门家长详情（官方即 GET，department_id 走 Query）
    /// <para>获取指定班级（部门）下的家长详情列表。</para>
    /// <para>官方业务限制：如需获取该部门及其子部门的所有家长，需先获取该部门下的子部门，
    /// 然后再获取子部门下的家长，逐层递归获取；官方无 cursor/limit 分页参数；
    /// 家长手机号第三方应用不可获取，代开发应用需管理员授权才返回。</para>
    /// </summary>
    /// <param name="departmentId">获取的部门 id（班级）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>家长详情列表（parents，含每个家长的孩子列表）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92446"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92627"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96741"/></para>
    /// </remarks>
    [Get("/cgi-bin/school/user/list_parent")]
    Task<SchoolDepartmentParentsResponse> ListDepartmentParentsAsync(
        [Query("department_id")] int departmentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置家校通讯录自动同步模式
    /// <para>修改家校通讯录与班级标签之间的自动同步模式。</para>
    /// <para>官方业务限制：<b>一旦设置禁止自动同步，将无法再次开启</b>（不可逆操作）。</para>
    /// </summary>
    /// <param name="request">设置请求体（<see cref="SchoolSetArchSyncModeRequest"/>：
    /// arch_sync_mode（1 禁止将标签同步至家校通讯录 / 2 禁止将家校通讯录同步至标签 /
    /// 3 禁止家校通讯录和标签相互同步））。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>设置结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92345"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92083"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100157"/></para>
    /// </remarks>
    [Post("/cgi-bin/school/set_arch_sync_mode")]
    Task<WechatWorkResponse> SetArchSyncModeAsync(
        [Body] SchoolSetArchSyncModeRequest request,
        CancellationToken cancellationToken = default);
}
