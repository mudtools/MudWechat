// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Wedoc;

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「文档」模块设置文档权限域公共 SDK
/// （获取文档权限信息 + 修改文档加入规则 + 修改文档成员与权限 + 修改文档安全设置
/// + 查询智能表格子表权限 + 更新智能表格子表权限 + 新增指定成员额外权限
/// + 更新指定成员额外权限 + 删除指定成员额外权限）。
/// <para>
/// 官方对三类应用开放完全一致的 9 个端点，全部收敛声明于本接口；应用类型子接口均为零差异端点空标记：
/// 企业自建应用见 <see cref="IWechatWorkInternalWedocDocPermissionService"/>，
/// 第三方应用见 <see cref="IWechatWorkThirdPartyWedocDocPermissionService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderWedocDocPermissionService"/>。
/// 官方「管理智能表格内容权限」一个文档页承载 5 个路由（<c>smartsheet/content_priv/</c> 前缀），故 6 个官方文档页对应 9 个端点。
/// </para>
/// <para>管理文档族见 <see cref="IWechatWorkWedocService"/>；
/// 管理文档内容族见 <see cref="IWechatWorkWedocDocumentService"/>；
/// 管理表格内容族见 <see cref="IWechatWorkWedocSpreadsheetService"/>；
/// 管理智能表格内容族见 <see cref="IWechatWorkWedocSmartSheetService"/>；
/// 管理智能文档内容族见 <see cref="IWechatWorkWedocSmartDocService"/>；
/// 管理收集表族见 <see cref="IWechatWorkWedocFormService"/>。</para>
/// </summary>
/// <remarks>
/// <para>
/// 三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>），
/// 由多应用基座按当前应用上下文（AppKey + scope）路由——第三方/代开发消费授权企业级 access_token（scope = authCorpId），
/// 须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// </para>
/// <para>权限口径：企业自建应用需使用配置到「可调用应用」列表中的应用 secret 所获取的 access_token 调用；
/// 第三方应用与服务商代开发（代开发自建应用）需具有「文档」权限。仅可操作该应用自己创建的文档。</para>
/// <para>官方业务限制（全集）：本域端点适用于文档、智能文档、表格、智能表格四类对象；
/// 文档需有至少一个管理员，才能将 <c>corp_internal_approve_only_by_admin</c> / <c>corp_external_approve_only_by_admin</c> 设为 <c>true</c>；
/// 修改文档成员与权限的两个成员列表批次大小均最大 100，且成员仅支持按人配置（type = 1）；
/// 智能表格内容权限由全员权限（每个智能表格有且只有一个）及至多 20 条成员额外权限组成，
/// 新增额外权限时权限规则名称不可重复，更新指定成员额外权限时成员最多可设置 50 个；
/// 修改文档加入规则的 <c>co_auth_list</c> 传入空列表会清空特定部门权限列表，<c>type</c> 目前仅支持部门（值 2）。</para>
/// <para>常见拼写陷阱（照抄官方原文，勿顺手修正）：修改文档安全设置官方路由为 <c>mod_doc_safty_setting</c>
/// （safty 疑为 safety 笔误）；获取文档权限信息响应的 <c>watermark.text</c> / <c>doc_member_list[].userid</c>
/// 官方参数表标注为 bytes，请求侧与示例均为字符串，本仓以字符串承载；
/// 子表权限 <c>priv_list[].priv</c> 官方参数表标注为 string 而请求/响应示例均为数字（如 <c>2</c>），以示例为准。</para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkWedocDocPermissionService
{
    /// <summary>
    /// 获取文档权限信息
    /// <para>该接口用于获取文档的权限信息，适用于文档、智能文档、表格、智能表格，
    /// 返回查看规则、文档通知范围及权限、安全设置信息。</para>
    /// <para>官方业务限制：仅可访问该应用创建的文档；
    /// 外部用户在不同文档中返回的 <c>tmp_external_userid</c> 不一致，须注意区分。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetWedocDocAuthRequest"/>：docid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>文档的查看规则（access_rule）/ 安全设置（secure_setting）/ 文档通知范围及权限列表（doc_member_list）/ 文档查看权限特定部门列表（co_auth_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用/第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97461"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97471"/></para>
    /// <para>官方业务限制：只能访问该应用创建的文档。</para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/doc_get_auth")]
    Task<GetWedocDocAuthResponse> GetDocAuthAsync(
        [Body] GetWedocDocAuthRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改文档加入规则
    /// <para>该接口用于修改文档、智能文档、表格、智能表格的加入规则（企业内/企业外成员浏览开关与权限、管理员审批、禁止外部分享、特定部门权限）。</para>
    /// <para>官方业务限制：文档需有至少一个管理员，才能将 <c>approve_only_by_admin</c> 类参数设置为 <c>true</c>；
    /// <c>enable_corp_internal</c> 为 <c>false</c> 时 <c>corp_internal_approve_only_by_admin</c> 只能为 <c>true</c>；
    /// <c>enable_corp_external</c> 与 <c>ban_share_external</c> 均为 <c>false</c> 时 <c>corp_external_approve_only_by_admin</c> 只能为 <c>true</c>；
    /// <c>update_co_auth_list</c> 为 <c>true</c> 时更新特定部门列表（传空列表则清空）；
    /// <c>co_auth_list[].type</c> 目前仅支持部门（值 2）。有值的字段才会覆盖原配置。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="UpdateWedocDocJoinRuleRequest"/>：docid / enable_corp_internal / corp_internal_auth / enable_corp_external / corp_external_auth / corp_internal_approve_only_by_admin / corp_external_approve_only_by_admin / ban_share_external / update_co_auth_list / co_auth_list）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用/第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97778"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97792"/></para>
    /// <para>官方业务限制：只能操作该应用创建的文档。</para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/mod_doc_join_rule")]
    Task<WechatWorkResponse> UpdateDocJoinRuleAsync(
        [Body] UpdateWedocDocJoinRuleRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改文档成员与权限
    /// <para>该接口用于管理文档、智能文档、表格、智能表格的成员，支持新增、删除成员及修改成员权限。</para>
    /// <para>官方业务限制：<c>update_file_member_list</c> 与 <c>del_file_member_list</c> 两个列表的批次大小均最大 100；
    /// 文档成员仅支持按人配置（type = 1）；外部成员以 <c>tmp_external_userid</c> 标识，同一用户在不同文档中该 id 不一致。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="UpdateWedocDocMemberRequest"/>：docid / update_file_member_list / del_file_member_list）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用/第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97781"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97795"/></para>
    /// <para>官方业务限制：只能操作该应用创建的文档；成员权限取值 1 只读、2 读写、7 管理员。</para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/mod_doc_member")]
    Task<WechatWorkResponse> UpdateDocMemberAsync(
        [Body] UpdateWedocDocMemberRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改文档安全设置
    /// <para>该接口用于修改文档、智能文档、表格、智能表格的安全设置（只读成员复制/下载开关与水印设置）。</para>
    /// <para>官方业务限制：接口创建的文档、表格默认禁止仅浏览成员发表评论，文档管理员可在文档的「权限设置」中修改；
    /// 有值的字段才会覆盖原配置。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="UpdateWedocDocSafetySettingRequest"/>：docid / enable_readonly_copy / watermark）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用/第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97782"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97797"/></para>
    /// <para>官方业务限制：只能操作该应用创建的文档。
    /// 官方路由拼写为 <c>mod_doc_safty_setting</c>（safty 疑为 safety 笔误），本 SDK 照抄官方原文。</para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/mod_doc_safty_setting")]
    Task<WechatWorkResponse> UpdateDocSaftySettingAsync(
        [Body] UpdateWedocDocSafetySettingRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询智能表格子表权限
    /// <para>该接口用于查询智能表格的内容权限规则（全员权限与成员额外权限）。</para>
    /// <para>官方业务限制：<c>type</c> 取 <c>1</c> 全员权限、<c>2</c> 额外权限，查询额外权限时须填 <c>rule_id_list</c>；
    /// 智能表格内容权限由全员权限（每个智能表格有且只有一个）及至多 20 条成员额外权限组成。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetWedocSheetPrivRequest"/>：docid / type / rule_id_list）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>权限规则列表（rule_list：rule_id / type / name / priv_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用/第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99935"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100193"/></para>
    /// <para>官方契约陷阱：<c>rule_id_list</c> 官方参数表为 uint32 数组，官方 JSON 示例以字符串占位（如 <c>"RULEID1"</c>），本 SDK 以参数表为准。</para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/smartsheet/content_priv/get_sheet_priv")]
    Task<GetWedocSheetPrivResponse> GetSheetPrivAsync(
        [Body] GetWedocSheetPrivRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新智能表格子表权限
    /// <para>该接口用于更新智能表格的内容权限规则（全员权限或成员额外权限，可按子表分别配置字段权限与记录权限）。</para>
    /// <para>官方业务限制：每个智能表格有且只有一个全员权限，<c>type</c> 为 <c>2</c>（额外权限）时 <c>rule_id</c> 必填；
    /// <c>priv_list[].priv</c> 为 <c>2</c>（可编辑）或 <c>3</c>（仅浏览）时 <c>record_priv</c> 必填；
    /// <c>field_priv.field_default_rule</c> 在 <c>field_range_type</c> 为 <c>1</c>（所有字段）时必填、为 <c>2</c> 时不可指定；
    /// <c>priv_list[].clear</c> 为 <c>true</c> 时清除该子表的设置并恢复默认权限；
    /// 记录条件仅对人员、单选、多选三种字段类型有效（<c>field_id</c> 为 <c>CREATED_USER</c> 时表示记录创建者）。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="UpdateWedocSheetPrivRequest"/>：docid / type / rule_id / name / priv_list）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用/第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99935"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100193"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/smartsheet/content_priv/update_sheet_priv")]
    Task<WechatWorkResponse> UpdateSheetPrivAsync(
        [Body] UpdateWedocSheetPrivRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 新增指定成员额外权限
    /// <para>该接口用于为智能表格新增一条成员额外权限规则。</para>
    /// <para>官方业务限制：权限规则名称不可重复；
    /// 智能表格内容权限由全员权限及至多 20 条成员额外权限组成。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="CreateWedocSheetPrivRuleRequest"/>：docid / name）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>新创建的成员权限规则 id（rule_id）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用/第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99935"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100193"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/smartsheet/content_priv/create_rule")]
    Task<CreateWedocSheetPrivRuleResponse> CreateSheetPrivRuleAsync(
        [Body] CreateWedocSheetPrivRuleRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新指定成员额外权限
    /// <para>该接口用于更新智能表格指定成员额外权限规则的成员范围（新增成员与移除成员）。</para>
    /// <para>官方业务限制：成员最多可设置 50 个。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="UpdateWedocSheetPrivRuleMemberRequest"/>：docid / rule_id / add_member_range / del_member_range）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用/第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99935"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100193"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/smartsheet/content_priv/mod_rule_member")]
    Task<WechatWorkResponse> UpdateSheetPrivRuleMemberAsync(
        [Body] UpdateWedocSheetPrivRuleMemberRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除指定成员额外权限
    /// <para>该接口用于删除智能表格的指定成员额外权限规则。</para>
    /// <para>官方业务限制：智能表格内容权限由全员权限及至多 20 条成员额外权限组成，全员权限不可删除。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="DeleteWedocSheetPrivRuleRequest"/>：docid / rule_id_list）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用/第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99935"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100193"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/smartsheet/content_priv/delete_rule")]
    Task<WechatWorkResponse> DeleteSheetPrivRuleAsync(
        [Body] DeleteWedocSheetPrivRuleRequest request,
        CancellationToken cancellationToken = default);
}
