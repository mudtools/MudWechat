// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OpenPlatform.Abstractions.Authentication;
using Mud.Wechat.OpenPlatform.DataModels;
using Mud.Wechat.OpenPlatform.DataModels.ComponentTcb;

namespace Mud.Wechat.OpenPlatform;

/// <summary>
/// 微信开放平台「云开发数据库管理」域 SDK（<b>9 端点</b>：聚合 / 集合管理 / 导出 / 获取权限 / 导入 / 索引管理 / 迁移状态 / 修改权限 / 记录管理）。
/// </summary>
/// <remarks>
/// <para><b>路由面</b>：全域 <b>50</b> 条路由恒为 <c>/componenttcb/*</c> 前缀、恒为 <b>POST</b>、恒以 Query <c>access_token</c> 承载 <c>component_access_token</c>（2026-10-15 逐页核验：每条路由的官方页正文请求地址与清单一致，零错配；无任何端点要求额外 Query 参数）。本接口承载其中 <b>9</b> 条。</para>
/// <para>令牌与开放面同 <see cref="IWechatOpenPlatformComponentTcbEnvService"/>：官方「第三方调用」明写仅 <c>component_access_token</c> 自己调用，全域无权限集 id、无端点级频率数字。</para>
/// <para><b>一个路由多动作</b>是本组官方形态：<c>dbcollection</c>（get/add/del）、<c>dbindex</c>（create/delete）、<c>dbrecord</c>（insert/delete/update/query）由 <c>action</c> 判别 ⇒ SDK 不拆端点、照官方单路由声明，动作枚举写在方法注释里。</para>
/// <para><b>权限面</b>：<c>dbgetacl</c>/<c>dbmodifyacl</c> 的 <c>acl_tag</c> 官方枚举为 <c>READONLY</c>（公有读私有写）/ <c>PRIVATE</c>（私有读写）/ <c>ADMINWRITE</c>（仅管理员可写）/ <c>ADMINONLY</c>（仅管理员可操作）/ <c>CUSTOM</c>（自定义规则）⇒ 放宽 <c>acl_tag</c> 等于放开终端用户直读写权限，属<b>安全面变更</b>。</para>
/// <para><b>数据明文</b>：<c>dbaggregate</c>/<c>dbrecord</c> 的应答 <c>data</c> 是业务记录数组（SDK 以 <c>List&lt;JsonElement&gt;</c> 原样承载、不解析）；<c>dbexport</c>/<c>dbmigrationstate</c> 的 <c>file_url</c> 是带签名临时链接 ⇒ 均<b>禁入日志、遥测与异常消息</b>。</para>
/// <para><c>dbimport</c> 的 <c>file_type</c>（1=JSON、2=CSV）与 <c>conflict_mode</c>（1/2）为官方枚举；导入是<b>写入客户环境数据</b>的操作，须由宿主显式确认集合归属。</para>
/// <para><b>落位</b>：<c>Interfaces/ComponentTcb/IWechatOpenPlatformComponentTcbDatabaseService.cs</c>；DTO 命名空间 <c>Mud.Wechat.OpenPlatform.DataModels.ComponentTcb</c>
/// （子目录 Env/Database/File/Scf/CloudRun/StaticStore 仅作组织）；<c>OpenPlatformModule.ComponentTcb</c>，注册组 <c>ComponentTcb</c>（六支接口共用一次注册）。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ComponentTcb", TokenManage = nameof(IOpenPlatformAppManager))]
[Token(TokenType = OpenPlatformTokenTypes.ComponentAccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWechatOpenPlatformComponentTcbDatabaseService
{
    // ==================== 云开发数据库管理（9 端点）====================

    /// <summary>
    /// 「数据库聚合」（<c>dbAggregate</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/db-mgnt/api_dbaggregate.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformComponentTcbAggregateDatabaseRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformComponentTcbAggregateDatabaseResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/componenttcb/dbaggregate</c>；JSON 请求体，Query 携带 <c>access_token</c>（值为 <c>component_access_token</c>）。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 component_access_token」；「第三方调用」段原文「本接口仅支持第三方平台使用 component_access_token 自己调用。」；「适用范围」段原文「本接口支持「第三方平台」账号类型代调用」；本接口不支持云调用。</para>
    /// <para>官方原文：「数据库操作语句说明：数据库操作语句语法与数据库 API相同， 聚合操作参考聚合文档」</para>
    /// <para>应答 <c>data</c> 为查询命中的<b>业务记录明文数组</b>（结构由集合 schema 决定，本 SDK 不解析）⇒ <b>可能含用户数据，禁入日志、遥测与异常消息</b>，落盘与脱敏由宿主决定。</para>
    /// <para>官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/componenttcb/dbaggregate")]
    Task<OpenPlatformComponentTcbAggregateDatabaseResponse> AggregateDatabaseAsync(
        [Body] OpenPlatformComponentTcbAggregateDatabaseRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「数据库集合管理」（<c>dbcollectionManage</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-common/db-management/api_dbcollectionmanage.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformComponentTcbManageDatabaseCollectionRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformComponentTcbManageDatabaseCollectionResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/componenttcb/dbcollection</c>；JSON 请求体，Query 携带 <c>access_token</c>（值为 <c>component_access_token</c>）。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 component_access_token」；「第三方调用」段原文「本接口仅支持第三方平台使用 component_access_token 自己调用。」；「适用范围」段原文「本接口支持「第三方平台」账号类型代调用」；本接口不支持云调用。</para>
    /// <para><b>官方枚举值</b>：<c>Body.action</c> = <c>get</c>（查询）、<c>add</c>（增加）、<c>del</c>（删除）。</para>
    /// <para>官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/componenttcb/dbcollection")]
    Task<OpenPlatformComponentTcbManageDatabaseCollectionResponse> ManageCollectionAsync(
        [Body] OpenPlatformComponentTcbManageDatabaseCollectionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「数据库导出」（<c>dbExport</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/db-mgnt/api_dbexport.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformComponentTcbExportDatabaseRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformComponentTcbExportDatabaseResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/componenttcb/dbexport</c>；JSON 请求体，Query 携带 <c>access_token</c>（值为 <c>component_access_token</c>）。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 component_access_token」；「第三方调用」段原文「本接口仅支持第三方平台使用 component_access_token 自己调用。」；「适用范围」段原文「本接口支持「第三方平台」账号类型代调用」；本接口不支持云调用。</para>
    /// <para>官方原文：「导出条件说明：查询语句语法与数据库 API相同。」</para>
    /// <para>应答 <c>file_url</c> 为<b>带签名的临时下载链接</b>，等同凭据 ⇒ 禁入日志、遥测与异常消息，且不得持久化到可公开访问的存储。</para>
    /// <para>官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/componenttcb/dbexport")]
    Task<OpenPlatformComponentTcbExportDatabaseResponse> ExportDatabaseAsync(
        [Body] OpenPlatformComponentTcbExportDatabaseRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「获取数据库操作权限」（<c>getPermission</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/db-mgnt/api_getpermission.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformComponentTcbGetDatabasePermissionRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformComponentTcbGetDatabasePermissionResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/componenttcb/dbgetacl</c>；JSON 请求体，Query 携带 <c>access_token</c>（值为 <c>component_access_token</c>）。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 component_access_token」；「第三方调用」段原文「本接口仅支持第三方平台使用 component_access_token 自己调用。」；「适用范围」段原文「本接口支持「第三方平台」账号类型代调用」；本接口不支持云调用。</para>
    /// <para><b>官方枚举值</b>：<c>acl_tag 枚举值</c> = <c>READONLY</c>（公有读，私有写）、<c>PRIVATE</c>（私有读写）、<c>ADMINWRITE</c>（仅管理员可写）、<c>ADMINONLY</c>（仅管理员可操作）、<c>CUSTOM</c>（自定义规则）。</para>
    /// <para>官方原文：「acl_tag 枚举值 值 | 说明 | READONLY | 公有读，私有写 | PRIVATE | 私有读写 | ADMINWRITE | 仅管理员可写 | ADMINONLY | 仅管理员可操作 | CUSTOM | 自定义规则 |」</para>
    /// <para>官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/componenttcb/dbgetacl")]
    Task<OpenPlatformComponentTcbGetDatabasePermissionResponse> GetDatabasePermissionAsync(
        [Body] OpenPlatformComponentTcbGetDatabasePermissionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「数据库导入」（<c>dbImport</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/db-mgnt/api_dbimport.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformComponentTcbImportDatabaseRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformComponentTcbImportDatabaseResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/componenttcb/dbimport</c>；JSON 请求体，Query 携带 <c>access_token</c>（值为 <c>component_access_token</c>）。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 component_access_token」；「第三方调用」段原文「本接口仅支持第三方平台使用 component_access_token 自己调用。」；「适用范围」段原文「本接口支持「第三方平台」账号类型代调用」；本接口不支持云调用。</para>
    /// <para><b>官方枚举值</b>：<c>file_type 的枚举值</c> = <c>1</c>（JSON）、<c>2</c>（CSV）；<c>conflict_mode 的枚举值</c> = <c>1</c>（INSERT）、<c>2</c>（UPSERT）。</para>
    /// <para>官方原文：「file_type 的枚举值 值 | 说明 | 1 | JSON | 2 | CSV | conflict_mode 的枚举值 值 | 说明 | 1 | INSERT | 2 | UPSERT |」</para>
    /// <para>官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/componenttcb/dbimport")]
    Task<OpenPlatformComponentTcbImportDatabaseResponse> ImportDatabaseAsync(
        [Body] OpenPlatformComponentTcbImportDatabaseRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「数据库索引管理」（<c>dbindexManage</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/db-mgnt/api_dbindexmanage.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformComponentTcbManageDatabaseIndexRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/componenttcb/dbindex</c>；JSON 请求体，Query 携带 <c>access_token</c>（值为 <c>component_access_token</c>）。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「ACCESS_TOKEN」；「第三方调用」段原文「本接口仅支持第三方平台使用 component_access_token 自己调用。」；「适用范围」段原文「本接口支持「第三方平台」账号类型代调用」；本接口不支持云调用。</para>
    /// <para><b>官方枚举值</b>：<c>Body.action</c> = <c>create</c>（创建索引）、<c>delete</c>（删除索引）；<c>Body.indexes(Array).keys.direction</c> = <c>1</c>（升序）、<c>-1</c>（降序）、<c>2dsphere</c>（地理位置）。</para>
    /// <para>官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/componenttcb/dbindex")]
    Task<OpenPlatformResponse> ManageIndexAsync(
        [Body] OpenPlatformComponentTcbManageDatabaseIndexRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「数据库迁移状态查询」（<c>getMigrationState</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/db-mgnt/api_getmigrationstate.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformComponentTcbGetDatabaseMigrationStateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformComponentTcbGetDatabaseMigrationStateResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/componenttcb/dbmigrationstate</c>；JSON 请求体，Query 携带 <c>access_token</c>（值为 <c>component_access_token</c>）。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 component_access_token」；「第三方调用」段原文「本接口仅支持第三方平台使用 component_access_token 自己调用。」；「适用范围」段原文「本接口支持「第三方平台」账号类型代调用」；本接口不支持云调用。</para>
    /// <para><b>敏感面</b>：<c>file_url</c> 属凭据、签名链接或环境内部信息 ⇒ <b>禁入日志、遥测与异常消息</b>。</para>
    /// <para>应答 <c>file_url</c> 为<b>带签名的临时链接</b> ⇒ 禁入日志、遥测与异常消息。</para>
    /// <para>官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/componenttcb/dbmigrationstate")]
    Task<OpenPlatformComponentTcbGetDatabaseMigrationStateResponse> GetMigrationStateAsync(
        [Body] OpenPlatformComponentTcbGetDatabaseMigrationStateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「修改数据库操作权限」（<c>setPermission</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/db-mgnt/api_setpermission.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformComponentTcbSetDatabasePermissionRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/componenttcb/dbmodifyacl</c>；JSON 请求体，Query 携带 <c>access_token</c>（值为 <c>component_access_token</c>）。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 component_access_token」；「第三方调用」段原文「本接口仅支持第三方平台使用 component_access_token 自己调用。」；「适用范围」段原文「本接口支持「第三方平台」账号类型代调用」；本接口不支持云调用。</para>
    /// <para><b>官方枚举值</b>：<c>acl_tag 枚举值</c> = <c>READONLY</c>（公有读，私有写）、<c>PRIVATE</c>（私有读写）、<c>ADMINWRITE</c>（仅管理员可写）、<c>ADMINONLY</c>（仅管理员可操作）、<c>CUSTOM</c>（自定义规则）。</para>
    /// <para>官方原文：「acl_tag 枚举值 值 | 说明 | READONLY | 公有读，私有写 | PRIVATE | 私有读写 | ADMINWRITE | 仅管理员可写 | ADMINONLY | 仅管理员可操作 | CUSTOM | 自定义规则 |」</para>
    /// <para>官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/componenttcb/dbmodifyacl")]
    Task<OpenPlatformResponse> SetDatabasePermissionAsync(
        [Body] OpenPlatformComponentTcbSetDatabasePermissionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「数据库记录管理」（<c>dbrecordManage</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/db-mgnt/api_dbrecordmanage.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformComponentTcbManageDatabaseRecordRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformComponentTcbManageDatabaseRecordResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/componenttcb/dbrecord</c>；JSON 请求体，Query 携带 <c>access_token</c>（值为 <c>component_access_token</c>）。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「ACCESS_TOKEN」；「第三方调用」段原文「本接口仅支持第三方平台使用 component_access_token 自己调用。」；「适用范围」段原文「本接口支持「第三方平台」账号类型代调用」；本接口不支持云调用。</para>
    /// <para><b>官方枚举值</b>：<c>Body.action</c> = <c>insert</c>（数据库插入记录）、<c>delete</c>（数据库删除记录）、<c>update</c>（数据库更新记录）、<c>query</c>（数据库查询记录）。</para>
    /// <para>一个路由承载四种动作（<c>insert</c>/<c>delete</c>/<c>update</c>/<c>query</c>），由 <c>action</c> 判别 ⇒ <c>data</c> 字段语义随动作变化；查询态的 <c>data</c> 为<b>记录明文数组</b>（本 SDK 不解析）⇒ <b>可能含用户数据，禁入日志、遥测与异常消息</b>。</para>
    /// <para>官方本页<b>未给端点级频率数字</b>。</para>
    /// </remarks>
    [Post("/componenttcb/dbrecord")]
    Task<OpenPlatformComponentTcbManageDatabaseRecordResponse> ManageRecordAsync(
        [Body] OpenPlatformComponentTcbManageDatabaseRecordRequest request,
        CancellationToken cancellationToken = default);

}
