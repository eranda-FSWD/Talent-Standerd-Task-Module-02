import React from "react";
import ReactDOM from "react-dom";
import Cookies from "js-cookie";
import TalentCard from "../TalentFeed/TalentCard.jsx";
import TalentDetail from "../TalentFeed/TalentDetail.jsx";
import { Loader } from "semantic-ui-react";
import CompanyProfile from "../TalentFeed/CompanyProfile.jsx";
import FollowingSuggestion from "../TalentFeed/FollowingSuggestion.jsx";
import { BodyWrapper, loaderData } from "../Layout/BodyWrapper.jsx";
import TalentCardDetail from "./TalentCardDetail.jsx";

export default class TalentFeed extends React.Component {
  constructor(props) {
    super(props);

    let loader = loaderData;
    loader.allowedUsers.push("Employer");
    loader.allowedUsers.push("Recruiter");

    this.state = {
      showDetails: false,
      loadNumber: 5,
      loadPosition: 0,
      feedData: [],
      watchlist: [],
      loaderData: loader,
      loadingFeedData: false,
      companyDetails: null,
    };

    this.init = this.init.bind(this);
    //this.updateWithoutSave = this.updateWithoutSave.bind(this);
    this.loadData = this.loadData.bind(this);
  }

  init() {
    let loaderData = TalentUtil.deepCopy(this.state.loaderData);
    loaderData.isLoading = false;
    this.setState({ loaderData }); //comment this
  }
  componentDidMount() {
    window.addEventListener("scroll", this.handleScroll);
    this.loadData();
    this.init();
  }

  /* updateWithoutSave(newValues) {
    let newProfile = Object.assign({}, this.state.feedData, newValues);
    this.setState({
      feedData: newProfile,
    });    
  } */

  loadData(callback) {
    var cookies = Cookies.get("talentAuthToken");
    $.ajax({
      url: "http://localhost:60290/profile/profile/getTalent",
      headers: {
        Authorization: "Bearer " + cookies,
        "Content-Type": "application/json",
      },
      type: "GET",
      data: {
        position: this.state.loadPosition,
        number: this.state.loadNumber,
      },

      contentType: "application/json",
      dataType: "json",
      success: function (res) {
        if (res.success) {
          console.log(res);
          console.log(this.state.loadPosition);
          var newFeeddata = [...this.state.feedData, ...res.data];
          console.log(newFeeddata);
          this.setState({
            feedData: newFeeddata,
          });
        }
      }.bind(this),
    });
    this.init();
  }

  handleScroll() {
    const win = $(window);

    if (
      $(document).height() - win.height() == Math.round(win.scrollTop()) ||
      $(document).height() - win.height() - Math.round(win.scrollTop()) == 1
    ) {
      $("#load-more-loading").show();
      let loaderData = TalentUtil.deepCopy(this.state.loaderData);
      loaderData.isLoading = false;
      this.loadData(() => this.setState({ loaderData }));
    }
  }

  render() {
    console.log("Feed Data :", this.state.feedData);
    return (
      <BodyWrapper reload={this.init} loaderData={this.state.loaderData}>
        <div className="ui grid talent-feed container">
          <div className="four wide column">
            <CompanyProfile />
          </div>
          <div className="eight wide column">
            <TalentCard profileDate={this.state.feedData} />
            <p id="load-more-loading">
              <img src="/images/rolling.gif" alt="Loading…" />
            </p>
          </div>
          <div className="four wide column">
            <div className="ui card">
              <FollowingSuggestion talents={this.state.feedData} />
            </div>
          </div>
        </div>
      </BodyWrapper>
    );
  }
}
