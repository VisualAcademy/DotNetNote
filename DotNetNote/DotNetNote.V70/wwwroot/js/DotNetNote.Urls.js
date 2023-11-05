/**
 * @target URL 관리 모듈
 */

/**
 * URL 관리 모듈 개체입니다.
 * 
 * @type {object}
 * @class urlManager
 * @namespace urlManager
 */
var urlManager = (function () {
    /**
     * URL 관리자 페이지
     */
    var popupPage = 1;

    /**
     * URL 관리자 전체 페이지 수
     */
    var popupCount = 0;

    return {
        /**
         * URL 목록을 반환합니다. 
         * 
         * @author PARK YONGJUNE
         * @date 2019. 04. 20.
         * @memberOf urlManager
         * @param {number} page - 페이지
         */
        renderUrlList: function (page) {
            try {
                if (!page || isNaN(page)) {
                    page = 1;
                }

                $.ajax({
                    url: "/UrlsServices/UrlIndex",
                    type: "POST",
                    data: "page=" + page + "&keyword=" + encodeURI($("#txtSearch").val()),
                    cache: false,
                    dataType: "json",
                    success: function (data, textStatus, jqXHR) {
                        var list = data.list;
                        var count = data.count;

                        // console.log(list);

                        // 페이지 정보
                        popupPage = page;
                        popupCount = Math.ceil(count / 5);
                        var startNum = (count - (popupPage - 1) * 5);

                        // 목록 생성
                        var listBase = $("#dnnMainIndex .tableWrap tbody");
                        var html = "";
                        listBase.html("");
                        for (var i = 0; i < list.length; i++) {
                            html += "<tr>";
                            html += "<td class='num'>" + (startNum - i) + "</td>";
                            html += "<td class='tit'><a href='javascript:urlManager.showPopupView(" + list[i].id + ");'>" + list[i].siteUrl + "</a></td>";
                            html += "<td class='text-center'>" + list[i].created + "</td>";
                            html += "<td class='text-center d-none d-sm-table-cell'>" + list[i].userName + "</td>";
                            html += "<td class='text-right text-nowrap'>" + "<a href='javascript:urlManager.modifyArticle(" + list[i].id + ");'>수정</a> <a href='javascript:urlManager.deleteArticle(" + list[i].id + ");'>삭제</a></td>";
                            html += "</tr>";
                        }
                        listBase.html(html);

                        // 페이징 처리
                        var pgStart = Math.floor((popupPage - 1) / 5) * 5 + 1;

                        html = "<button class='prev2 first'>처음</button> <button class='prev'>이전</button> ";
                        for (var j = 0; j < 5; j++) {
                            var cur = pgStart + j;
                            if (cur > popupCount) {
                                break;
                            }
                            if (popupPage === cur) {
                                html += "<a href='#' class='on'>" + cur + "</a> ";
                            } else {
                                html += "<a href='javascript:urlManager.renderUrlList(" + cur + ");'>" + cur + "</a> ";
                            }
                        }
                        html += "<button class='next'>다음</button> <button class='next2 last'>끝</button>";

                        $("#dnnMainIndex .page").html(html);
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        console.log("URL 목록을 가져오던 중, 알 수 없는 오류가 발생했습니다.");
                    }
                });
            } catch (e) {
                console.log("urlManager.renderUrlList: " + e);
            }
        },
        /**
         * 게시판 페이지의 팝업 공지사항 목록을 이동합니다.
         * 
         * @author PARK YONGJUNE
         * @date 2019. 04. 20.
         * @memberOf urlManager
         * @param {object} event - event 개체
         */
        moveUrlList: function (event) {
            var $btn = $(event.target);
            var pgStart = Math.floor((popupPage - 1) / 5) * 5 + 1;

            if ($btn.hasClass("prev2")) {
                popupPage = 1;
            } else if ($btn.hasClass("prev")) {
                popupPage--;
                if (popupPage < 1) {
                    popupPage = 1;
                }
            } else if ($btn.hasClass("next")) {
                popupPage++;
                if (popupPage > popupCount) {
                    popupPage = popupCount;
                }
            } else if ($btn.hasClass("next2")) {
                popupPage = popupCount;
            }

            urlManager.renderUrlList(popupPage);
        },
        /**
         * 좌측 팝업 목록을 출력합니다.
         * 
         * @author PARK YONGJUNE
         * @date 2019. 04. 20.
         * @memberOf urlManager
         * @param {object} event - event 개체
         */
        showPopupList: function (event) {
            $("#divDotNetNoteIndex").css("display", "block");
            $("#divDotNetNoteView").css("display", "none");

            $("#dnnMainIndex").addClass("on");
            $("#dim").show();
        },
        /**
         * 도메인 관리자 상세를 출력합니다.
         * 
         * @author PARK YONGJUNE
         * @date 2019. 04. 20.
         * @memberOf urlManager
         * @param {number} num - 도메인 관리자 상세 일련번호
         * @param {boolean} clearFilter - 검색 조건 초기화 여부
         */
        showPopupView: function (num, clearFilter) {

            if (clearFilter !== null && clearFilter === true) {
                $("#txtSearch").val("");
            }

            $("#divDotNetNoteIndex").css("display", "none");
            $("#divDotNetNoteView").css("display", "block");

            try {
                $.ajax({
                    url: "/UrlsServices/UrlDetails",
                    type: "POST",
                    data: "id=" + num + "&keyword=" + encodeURI($("#txtSearch").val()),
                    cache: false,
                    dataType: "json",
                    success: function (data, textStatus, jqXHR) {
                        $("#dnnMainIndex .view > .tit").html(data.siteUrl + "<span>" + data.created + "</span>");
                        $("#dnnMainIndex .view > .cont").html(data.content);

                        //if (data.fileName !== null && data.fileName.length > 0) {
                        //    $("#dnnMainIndex .view > .fileWrap > .fileCont").html("<a href='/Login/Download/?num=" + num + "'>" + data.fileName + "</a>");
                        //} else {
                        //    $("#dnnMainIndex .view > .fileWrap > .fileCont").html("<a href='#'>N/A</a>");
                        //}

                        if (data.nextTitle === "") {
                            $("#dnnMainIndex .view > .util > .next").html("<a href='#'>Next article does not exist.</a>");
                        } else {
                            $("#dnnMainIndex .view > .util > .next").html("<a href='javascript:urlManager.showPopupView(" + data.nextNum + ");'>" + data.nextTitle + "</a>");
                        }

                        if (data.prevTitle === "") {
                            $("#dnnMainIndex .view > .util > .prev").html("<a href='#'>Prev. article does not exist.</a>");
                        } else {
                            $("#dnnMainIndex .view > .util > .prev").html("<a href='javascript:urlManager.showPopupView(" + data.prevNum + ");'>" + data.prevTitle + "</a>");
                        }
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        console.log("URL 상세정보를 가져오던 중, 알 수 없는 오류가 발생했습니다.");
                    }
                });
            } catch (e) {
                console.log("urlManager.showPopupView: " + e);
            }

            //$("#dnnMainIndex").addClass("on");
            //$("#dim").show();
        },
        /**
         * 선택한 도메인을 삭제합니다. 
         * 
         * @author PARK YONGJUNE
         * @date 2019. 04. 20.
         * @memberOf urlManager
         * @param {number} id - URL 삭제
         */
        deleteArticle: function (id) {

            if (window.confirm("선택한 항목을 삭제하시겠습니까?")) {
                try {
                    $.ajax({
                        url: "/UrlsServices/DeleteUrlById",
                        type: "POST",
                        data: "id=" + id,
                        cache: false,
                        dataType: "json",
                        success: function (data, textStatus, jqXHR) {

                            console.log(data.message);
                            urlManager.renderUrlList(1); // 리스트 출력

                        },
                        error: function (jqXHR, textStatus, errorThrown) {
                            console.log("URL 삭제 중, 알 수 없는 오류가 발생했습니다.");
                        }
                    });
                } catch (e) {
                    console.log("urlManager.deleteArticle: " + e);
                }
            }
            else {
                return;
            }

        },
        /**
         * 선택한 아티클을 수정합니다. 
         * 
         * @author PARK YONGJUNE
         * @date 2019. 04. 20.
         * @memberOf urlManager
         * @param {number} id - 아티클 수정
         */
        modifyArticle: function (id) {

            $("#divId").show();
            $("#btnSave").text("수정");
            $("#lblSaveOrUpdate").text("수정");
            $("#dnnCreateForm").modal('show');

            try {
                $.ajax({
                    url: "/UrlsServices/UrlDetails",
                    type: "POST",
                    data: "id=" + id + "&keyword=" + encodeURI($("#txtSearch").val()),
                    cache: false,
                    dataType: "json",
                    success: function (data, textStatus, jqXHR) {

                        $("#id").val(id);
                        $("#userName").val(data.userName);
                        $("#siteUrl").val(data.siteUrl);
                        $("#content").val(data.content);

                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        console.log("URL 상세정보를 가져오던 중, 알 수 없는 오류가 발생했습니다.");
                    }
                });
            } catch (e) {
                console.log("urlManager.modifyArticle: " + e);
            }

        }
    };
})();
