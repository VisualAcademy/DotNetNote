// TODO:
export default {
    props: {
        pageNumber: Number,
        pageSize: Number,
        pagerButtonCount: Number,
        recordCount: Number,

        pageIndexChanged: Object
    },
    data() {
        return {
        };
    },
    computed: {
        // 총 페이지 수 
        pageCount: function() {
            return Math.ceil(this.recordCount / this.pageSize); 
        },
        // 페이지 인덱스
        pageIndex: function() {
            return this.pageNumber - 1; 
        },
        // 시작 페이지
        start: function() {
            return parseInt(this.pageIndex / this.pagerButtonCount) * this.pagerButtonCount + 1; //[?] 
        },
        // 끝 페이지 
        end: function() {
            return (parseInt(this.pageIndex / this.pagerButtonCount) + 1) * this.pagerButtonCount; //[?]
        },

        // 이전 n개
        prevN: function() {
            return parseInt((this.pageNumber - 1) / this.pagerButtonCount) * this.pagerButtonCount; // 이전 n개 페이지 번호 계산
        },
        prev: function() {
            return this.pageNumber - 1; // 이전 페이지 번호 계산
        },
        next: function() {
            return this.pageNumber + 1; // 이전 페이지 번호 계산
        },
        nextN: function() {
            return parseInt(this.pageIndex / this.pagerButtonCount) * this.pagerButtonCount + this.pagerButtonCount + 1; // 다음 n개 페이지 번호 계산
        },
        

        buttons: function () {
            const btns = [];
            for (let i = this.start; i <= this.end; i++) {
                btns.push(i); 
            }
            return btns; 
        }, 

        // Simple Paging 
        pages: function () {
            const list = [];
            for (let i = this.first; i <= this.last; i++) { 
                list.push(i); 
            }
            return list;
        },
        first() {
            return parseInt(this.pageNumber / 5) * 5 + 1;
        },
        last() {
            let lastPage = parseInt(this.pageNumber / 5) * 5 + 5;
            return lastPage <= this.pageCount ? lastPage : this.pageCount;
        }
    },
    methods: {
        pagerButtonClicked(pageNumber, e) {
            e.preventDefault();
            console.log("Pager Component", pageNumber);
            this.pageIndexChanged(pageNumber - 1); // PageIndex를 부모 컴포넌트로 전송
        },
        pagerButtonClickedSimple(pageNumber, e) {
            e.preventDefault();
            if (pageNumber < 0 || pageNumber > this.pageCount) {
                return;
            }
            //console.log("Pager Component", pageNumber);
            this.pageIndexChanged(pageNumber - 1); // PageIndex를 부모 컴포넌트로 전송
        }
    },

    template: `
<h3>고급 페이징</h3>
<div class="d-flex">
    <ul class="pagination pagination-sm mx-auto">

    <li v-if="pageNumber === 1" class="page-item" :key="first">
        <a href="#first" class="page-link first btn disabled"><span style="font-size: '7pt';">FIRST</span></a>
    </li>
    <li v-if="pageNumber !== 1" class="page-item" :key="first">
        <a href="#first" class="page-link first btn" v-on:click="pagerButtonClicked(1, $event)"><span style="font-size: '7pt';">FIRST</span></a>
    </li>

    <li v-if="pageNumber > pagerButtonCount" class="page-item" :key="prevN">
        <a href="#prevN" class="page-link prev btn" v-on:click="pagerButtonClicked(prevN, $event)">
            <span style="font-size: '7pt';">-{{pagerButtonCount}}</span>
        </a>
    </li>
    <li v-if="pageNumber <= pagerButtonCount" class="page-item" :key="prevN">
        <a href="#prevN" class="page-link prev btn disabled">
            <span style="font-size: '7pt'; ">-{{pagerButtonCount}}</span>
        </a>
    </li>

    <li v-if="pageNumber > 1" class="page-item" :key="prev">
        <a href="#prev" class="page-link prev btn" v-on:click="pagerButtonClicked(prev, $event)">
            <span style="font-size: '7pt'; ">PREV</span>
        </a>
    </li>
    <li v-if="pageNumber <= 1" class="page-item" :key="prev">
        <a href="#prev" class="page-link prev btn disabled">
            <span style="font-size: '7pt'; ">PREV</span>
        </a>
    </li>


    <li v-for="(btn, index) in buttons" class="page-item" :key="index">
        <a v-if="btn === pageNumber" href="#currentNumber" class="page-link current btn disabled"><span style=" font-size: '7pt'; ">{{btn}}</span></a>
        <a v-if="btn !== pageNumber && btn <= pageCount" href="#currentNumber" class="page-link current btn" v-on:click="pagerButtonClicked(btn, $event)"><span style=" font-size: '7pt'; ">{{btn}}</span></a>
    </li>


    <li v-if="pageNumber < pageCount" className="page-item" :key="next">
        <a href={"next"} class="page-link next btn" v-on:click="pagerButtonClicked(next, $event)">
            <span style="font-size: '7pt';">NEXT</span>
        </a>
    </li>
    <li v-if="pageNumber >= pageCount" className="page-item" :key="next">
        <a href={"next"} class="page-link next btn disabled">
            <span style="font-size: '7pt';">NEXT</span>
        </a>
    </li>

    <!-- 다음 n 개 링크 --> 
    <li v-if="end <= pageCount" class="page-item" :key="nextN">
        <a href="#nextN" class="page-link next btn" v-on:click="pagerButtonClicked(nextN, $event)">
            <span style="font-size: '7pt'; ">{{pagerButtonCount}}+</span>
        </a>
    </li>
    <li v-if="end > pageCount" class="page-item" :key="nextN">
        <a href="#nextN" class="page-link next btn disabled">
            <span style="font-size: '7pt'; ">{{pagerButtonCount}}+</span>
        </a>
    </li>


    <li v-if="pageNumber !== pageCount" class="page-item" key="last">
        <a href="#last" class="page-link last btn" v-on:click="pagerButtonClicked(pageCount, $event)"><span style="font-size: '7pt';">LAST</span></a>
    </li>
    <li v-if="pageNumber === pageCount" class="page-item" key="last">
        <a href="#last" class="page-link last btn disabled"><span style="font-size: '7pt'">LAST</span></a>
    </li>

    </ul>
</div>

<h3>심플 페이징</h3>
<div class="d-flex">
    <div>
        <a @click="pagerButtonClickedSimple(pageNumber - 1, $event)">&lt;</a>
        <a v-for="(lnk, index) in pages" :key="index" @click="pagerButtonClickedSimple(lnk, $event)" :class="lnk === pageNumber ? 'current' : ''">{{ lnk }}</a>
        <a @click="pagerButtonClickedSimple(pageNumber + 1, $event)">&gt;</a>
    </div>
</div>
  `
}
