// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.MiniProgram.DataModels.CloudBase;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「云开发（TCB）」域 SDK（<b>16 端点</b>：集合管理 3 + 记录增删改查与统计 5 + 聚合 1 + 索引 1 + 导入导出与状态 3 + 云存储上传/下载/删除链接 3）。
/// </summary>
/// <remarks>
/// <para><b>官方文档面</b>：本域路由的官方页面分布在两处文档树 —— 小程序自研面
/// <c>miniprogram/dev/server/API/cloudbase/{functions,database,storage,others}/*</c>，
/// 公众平台通用面（第三方/自研同页）<c>doc/oplatform/openApi/cloudbase-common/*</c> 与
/// <c>cloudbase-batch/*</c>；两处<b>路由同为 <c>/tcb/*</c></b>（2026-10-15 逐页核验）。
/// 逐端点官方链接见各方法注释。</para>
/// <para><b>令牌形态（与 MP-X2 / Vip 域同口径）</b>：全域使用<b>小程序 access_token</b>（Query 注入，
/// <c>MpTokenTypes.AccessToken</c>）。官方各页令牌说明给出「<c>access_token</c> / <c>authorizer_access_token</c>」
/// 或仅 <c>authorizer_access_token</c>（第三方代调用面）⇒ 本 SDK <b>不新增令牌类型</b>、也不声明
/// <c>component_access_token</c>；第三方代调用形态由宿主以小程序应用配置供给令牌。
/// <c>/componenttcb/*</c>（仅 <c>component_access_token</c> 的批量代云开发面）<b>属开放平台线</b>，
/// 本域不得声明（守卫 CBZ 锁定两线路由不交叉）。</para>
/// <para><b>权限集</b>：44 端点中 40 支官方标注权限集 id <b>49</b>（小程序云开发管理），
/// <c>/tcb/wxpaylist</c> 与 <c>/tcb/wxpayopenauth</c> 为 <b>49、102</b>，<c>/wxa/getopendata</c> 为 <b>18、49</b>，
/// <c>/wxa/getvoipsign</c> 为 <b>18</b>。全部端点<b>支持第三方平台代商家调用</b>。</para>
/// <para><b>端点级频率数字</b>：官方仅对 <c>/tcb/getqcloudtoken</c> 给出「<b>10 次每小时</b>」
///（超限提示可经 <c>clear_quota</c> 恢复额度）；其余 43 端点<b>未给任何数字</b> ⇒ 注释一律留档该空缺，
/// 不得凭第三方 SDK 注释或记忆补数。</para>
/// <para><b>敏感面</b>：<c>/tcb/getqcloudtoken</c>（<c>secretid</c>/<c>secretkey</c>/<c>token</c>）、
/// <c>/tcb/getcodesecret</c>（<c>codesecret</c>）、<c>/tcb/getuploadsignature</c>（签名 <c>headers</c>）、
/// <c>/tcb/uploadfile</c>（COS 直传 <c>authorization</c>/<c>token</c>）、
/// <c>/tcb/{set,get}callbackconfig</c> 与 <c>/tcb/{get,upload}funcconfig</c>（配置含环境变量）、
/// <c>/wxa/getopendata</c>（用户开放数据明文）⇒ 这些字段<b>禁入日志、遥测与异常消息</b>。</para>
/// <para><b>官方拼写照抄</b>：<c>getFuntionLink</c> / <c>getFuntionList</c> / <c>getFuntionConfig</c> /
/// <c>getUploadFuntionConfig</c> / <c>getYploadSignature</c> 为官方原文拼写（非笔误修正对象），
/// 方法名与英文接口名一律照抄。</para>
/// <para><b>落位</b>：<c>Interfaces/CloudBase/IWxaCloudBaseDataService.cs</c>；DTO 命名空间
/// <c>Mud.Wechat.MiniProgram.DataModels.CloudBase</c>（子目录 Env/Database/Storage/Scf/Misc 仅作组织）；
/// <c>MiniProgramModule.CloudBase</c>。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "CloudBase", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaCloudBaseDataService
{
    // ==================== 云存储（3 端点）====================

    /// <summary>
    /// 「删除文件」（<c>deleteTcbCloudFile</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-common/file-management/api_deletetcbcloudfile.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaCloudBaseDeleteTcbCloudFileRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="WxaCloudBaseDeleteTcbCloudFileResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/tcb/batchdeletefile</c>；JSON 请求体，Query 携带 <c>access_token</c>。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 access_token、authorizer_access_token」，权限集 id <b>49</b>，支持第三方平台代商家调用。官方明写本接口不支持云调用。</para>
    /// <para><b>删除为不可逆操作</b>：应答的 <c>delete_list</c> 逐文件给出 <c>status</c>/<c>errmsg</c>，部分成功是常态，宿主不得把「HTTP 成功」当作「全部删除成功」。</para>
    /// </remarks>
    [Post("/tcb/batchdeletefile")]
    Task<WxaCloudBaseDeleteTcbCloudFileResponse> DeleteTcbCloudFileAsync(
        [Body] WxaCloudBaseDeleteTcbCloudFileRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「获取文件下载链接」（<c>getDownloadTcbFileLink</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-common/file-management/api_getdownloadtcbfilelink.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaCloudBaseGetDownloadTcbFileLinkRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="WxaCloudBaseGetDownloadTcbFileLinkResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/tcb/batchdownloadfile</c>；JSON 请求体，Query 携带 <c>access_token</c>。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 access_token、authorizer_access_token」，权限集 id <b>49</b>，支持第三方平台代商家调用。官方明写本接口不支持云调用。</para>
    /// <para><c>file_list</c> 逐项携带 <c>max_age</c>（该文件下载链接的有效期秒数），返回的 <c>download_url</c> 为带签名的临时链接 ⇒ 不得持久化或对外长期暴露。</para>
    /// <para>逐文件给出 <c>status</c>/<c>errmsg</c>，<b>部分成功是常态</b>：HTTP 成功不等于全部文件都取到链接。</para>
    /// </remarks>
    [Post("/tcb/batchdownloadfile")]
    Task<WxaCloudBaseGetDownloadTcbFileLinkResponse> GetDownloadTcbFileLinkAsync(
        [Body] WxaCloudBaseGetDownloadTcbFileLinkRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「获取文件上传链接」（<c>getUploadTcbFileLink</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-common/file-management/api_getuploadtcbfilelink.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaCloudBaseGetUploadTcbFileLinkRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="WxaCloudBaseGetUploadTcbFileLinkResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/tcb/uploadfile</c>；JSON 请求体，Query 携带 <c>access_token</c>。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 access_token、authorizer_access_token」，权限集 id <b>49</b>，支持第三方平台代商家调用。官方明写本接口不支持云调用。</para>
    /// <para><b>敏感</b>：应答含 <c>authorization</c>/<c>token</c>/<c>cos_key</c>/<c>url</c> 等 COS 直传凭证（有效期短），<b>禁入日志与遥测</b>；凭证仅对「本次 <c>path</c>」有效，换路径须重新获取。</para>
    /// </remarks>
    [Post("/tcb/uploadfile")]
    Task<WxaCloudBaseGetUploadTcbFileLinkResponse> GetUploadTcbFileLinkAsync(
        [Body] WxaCloudBaseGetUploadTcbFileLinkRequest request,
        CancellationToken cancellationToken = default);

    // ==================== 数据库（13 端点）====================

    /// <summary>
    /// 「数据库插入记录」（<c>addDatabaseItem</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-common/db-management/api_adddatabaseitem.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaCloudBaseAddDatabaseItemRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="WxaCloudBaseAddDatabaseItemResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/tcb/databaseadd</c>；JSON 请求体，Query 携带 <c>access_token</c>。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 authorizer_access_token」，权限集 id <b>49</b>，支持第三方平台代商家调用。官方明写本接口不支持云调用。</para>
    /// <para>官方本页「注意事项」段为「本接口无特殊注意事项」，且未给端点级频率数字。</para>
    /// </remarks>
    [Post("/tcb/databaseadd")]
    Task<WxaCloudBaseAddDatabaseItemResponse> AddDatabaseItemAsync(
        [Body] WxaCloudBaseAddDatabaseItemRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「数据库聚合」（<c>aggregateDatabase</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-common/db-management/api_aggregatedatabase.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaCloudBaseAggregateDatabaseRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="WxaCloudBaseAggregateDatabaseResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/tcb/databaseaggregate</c>；JSON 请求体，Query 携带 <c>access_token</c>。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 authorizer_access_token」，权限集 id <b>49</b>，支持第三方平台代商家调用。官方明写本接口不支持云调用。</para>
    /// <para><c>agg</c> 为聚合操作语句（语法同数据库 API），返回 <c>data</c> 同样以 <c>List&lt;JsonElement&gt;?</c> 承载。</para>
    /// </remarks>
    [Post("/tcb/databaseaggregate")]
    Task<WxaCloudBaseAggregateDatabaseResponse> AggregateDatabaseAsync(
        [Body] WxaCloudBaseAggregateDatabaseRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「新增集合」（<c>addDatabaseCollection</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-common/db-management/api_adddatabasecollection.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaCloudBaseAddDatabaseCollectionRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/tcb/databasecollectionadd</c>；JSON 请求体，Query 携带 <c>access_token</c>。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 authorizer_access_token」，权限集 id <b>49</b>，支持第三方平台代商家调用。官方明写本接口不支持云调用。</para>
    /// <para>官方本页「注意事项」段为「本接口无特殊注意事项」，且未给端点级频率数字。</para>
    /// </remarks>
    [Post("/tcb/databasecollectionadd")]
    Task<WxaResponse> AddDatabaseCollectionAsync(
        [Body] WxaCloudBaseAddDatabaseCollectionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「删除集合」（<c>deleteDatabaseCollection</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-common/db-management/api_deletedatabasecollection.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaCloudBaseDeleteDatabaseCollectionRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/tcb/databasecollectiondelete</c>；JSON 请求体，Query 携带 <c>access_token</c>。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 authorizer_access_token」，权限集 id <b>49</b>，支持第三方平台代商家调用。官方明写本接口不支持云调用。</para>
    /// <para><b>删除集合连带其中全部记录</b>，不可逆；应答仅 <c>errcode</c>/<c>errmsg</c>。</para>
    /// </remarks>
    [Post("/tcb/databasecollectiondelete")]
    Task<WxaResponse> DeleteDatabaseCollectionAsync(
        [Body] WxaCloudBaseDeleteDatabaseCollectionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「获取集合信息」（<c>getDatabaseCollection</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-common/db-management/api_getdatabasecollection.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaCloudBaseGetDatabaseCollectionRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="WxaCloudBaseGetDatabaseCollectionResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/tcb/databasecollectionget</c>；JSON 请求体，Query 携带 <c>access_token</c>。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 authorizer_access_token」，权限集 id <b>49</b>，支持第三方平台代商家调用。官方明写本接口不支持云调用。</para>
    /// <para>官方本页「注意事项」段为「本接口无特殊注意事项」，且未给端点级频率数字。</para>
    /// </remarks>
    [Post("/tcb/databasecollectionget")]
    Task<WxaCloudBaseGetDatabaseCollectionResponse> GetDatabaseCollectionAsync(
        [Body] WxaCloudBaseGetDatabaseCollectionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「统计集合记录数」（<c>getDatabaseCount</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-common/db-management/api_getdatabasecount.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaCloudBaseGetDatabaseCountRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="WxaCloudBaseGetDatabaseCountResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/tcb/databasecount</c>；JSON 请求体，Query 携带 <c>access_token</c>。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 authorizer_access_token」，权限集 id <b>49</b>，支持第三方平台代商家调用。官方明写本接口不支持云调用。</para>
    /// <para>官方本页「注意事项」段为「本接口无特殊注意事项」，且未给端点级频率数字。</para>
    /// </remarks>
    [Post("/tcb/databasecount")]
    Task<WxaCloudBaseGetDatabaseCountResponse> GetDatabaseCountAsync(
        [Body] WxaCloudBaseGetDatabaseCountRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「数据库删除记录」（<c>deleteDatabaseItem</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-common/db-management/api_deletedatabaseitem.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaCloudBaseDeleteDatabaseItemRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="WxaCloudBaseDeleteDatabaseItemResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/tcb/databasedelete</c>；JSON 请求体，Query 携带 <c>access_token</c>。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 authorizer_access_token」，权限集 id <b>49</b>，支持第三方平台代商家调用。官方明写本接口不支持云调用。</para>
    /// <para><b>删除语句不可逆</b>，官方原文「数据库操作语句语法与数据库 API 相同」⇒ 语句正确但命中 0 条时 <c>deleted</c> 为 0，不报错。</para>
    /// </remarks>
    [Post("/tcb/databasedelete")]
    Task<WxaCloudBaseDeleteDatabaseItemResponse> DeleteDatabaseItemAsync(
        [Body] WxaCloudBaseDeleteDatabaseItemRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「数据库导出」（<c>exportDatabaseItem</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-common/db-management/api_exportdatabaseitem.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaCloudBaseExportDatabaseItemRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="WxaCloudBaseExportDatabaseItemResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/tcb/databasemigrateexport</c>；JSON 请求体，Query 携带 <c>access_token</c>。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 authorizer_access_token」，权限集 id <b>49</b>，支持第三方平台代商家调用。官方明写本接口不支持云调用。</para>
    /// <para>导出为<b>异步任务</b>：本接口仅返回 <c>job_id</c>，进度与结果文件地址须轮询 <see cref="IWxaCloudBaseDataService.GetDatabaseMigrateStatusAsync"/>。</para>
    /// </remarks>
    [Post("/tcb/databasemigrateexport")]
    Task<WxaCloudBaseExportDatabaseItemResponse> ExportDatabaseItemAsync(
        [Body] WxaCloudBaseExportDatabaseItemRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「数据库导入」（<c>importDatabaseItem</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-common/db-management/api_importdatabaseitem.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaCloudBaseImportDatabaseItemRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="WxaCloudBaseImportDatabaseItemResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/tcb/databasemigrateimport</c>；JSON 请求体，Query 携带 <c>access_token</c>。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 authorizer_access_token」，权限集 id <b>49</b>，支持第三方平台代商家调用。官方明写本接口不支持云调用。</para>
    /// <para>导入为<b>异步任务</b>：本接口仅返回 <c>job_id</c>，须轮询迁移状态；<c>create_collection</c> 与 <c>insertion_on_conflict</c> 决定冲突处置，配置错误会静默按官方默认执行。</para>
    /// </remarks>
    [Post("/tcb/databasemigrateimport")]
    Task<WxaCloudBaseImportDatabaseItemResponse> ImportDatabaseItemAsync(
        [Body] WxaCloudBaseImportDatabaseItemRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「数据库迁移状态查询」（<c>getDatabaseMigrateStatus</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-common/db-management/api_getdatabasemigratestatus.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaCloudBaseGetDatabaseMigrateStatusRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="WxaCloudBaseGetDatabaseMigrateStatusResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/tcb/databasemigratequeryinfo</c>；JSON 请求体，Query 携带 <c>access_token</c>。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 authorizer_access_token」，权限集 id <b>49</b>，支持第三方平台代商家调用。官方明写本接口不支持云调用。</para>
    /// <para>官方本页「注意事项」段为「本接口无特殊注意事项」，且未给端点级频率数字。</para>
    /// </remarks>
    [Post("/tcb/databasemigratequeryinfo")]
    Task<WxaCloudBaseGetDatabaseMigrateStatusResponse> GetDatabaseMigrateStatusAsync(
        [Body] WxaCloudBaseGetDatabaseMigrateStatusRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「数据库查询记录」（<c>getDatabaseRecord</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-common/db-management/api_getdatabaserecord.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaCloudBaseGetDatabaseRecordRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="WxaCloudBaseGetDatabaseRecordResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/tcb/databasequery</c>；JSON 请求体，Query 携带 <c>access_token</c>。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 authorizer_access_token」，权限集 id <b>49</b>，支持第三方平台代商家调用。官方明写本接口不支持云调用。</para>
    /// <para>官方原文「query 中应使用 <c>limit()</c> 限制单次拉取的数量，默认 10 条」；<c>data</c> 为记录 JSON 字符串列表，结构由集合自身决定 ⇒ 以 <c>List&lt;JsonElement&gt;?</c> 承载，不做类型推断。</para>
    /// </remarks>
    [Post("/tcb/databasequery")]
    Task<WxaCloudBaseGetDatabaseRecordResponse> GetDatabaseRecordAsync(
        [Body] WxaCloudBaseGetDatabaseRecordRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「数据库更新记录」（<c>updateDatabaseRecord</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-common/db-management/api_updatedatabaserecord.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaCloudBaseUpdateDatabaseRecordRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="WxaCloudBaseUpdateDatabaseRecordResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/tcb/databaseupdate</c>；JSON 请求体，Query 携带 <c>access_token</c>。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 authorizer_access_token」，权限集 id <b>49</b>，支持第三方平台代商家调用。官方明写本接口不支持云调用。</para>
    /// <para>官方本页「注意事项」段为「本接口无特殊注意事项」，且未给端点级频率数字。</para>
    /// </remarks>
    [Post("/tcb/databaseupdate")]
    Task<WxaCloudBaseUpdateDatabaseRecordResponse> UpdateDatabaseRecordAsync(
        [Body] WxaCloudBaseUpdateDatabaseRecordRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「更新数据库索引」（<c>updateDatabaseIndex</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-common/db-management/api_updatedatabaseindexs.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaCloudBaseUpdateDatabaseIndexRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/tcb/updateindex</c>；JSON 请求体，Query 携带 <c>access_token</c>。</para>
    /// <para><b>令牌与开放面</b>：官方本页令牌说明「接口调用凭证，可使用 authorizer_access_token」，权限集 id <b>49</b>，支持第三方平台代商家调用。官方明写本接口不支持云调用。</para>
    /// <para><b>索引变更不可逆且为整体提交</b>：<c>create_indexes</c> 与 <c>drop_indexes</c> 同批生效（删索引连带影响线上查询性能）。索引 <c>keys</c> 的顺序即复合索引顺序；应答仅 <c>errcode</c>/<c>errmsg</c>。</para>
    /// </remarks>
    [Post("/tcb/updateindex")]
    Task<WxaResponse> UpdateDatabaseIndexAsync(
        [Body] WxaCloudBaseUpdateDatabaseIndexRequest request,
        CancellationToken cancellationToken = default);

}
