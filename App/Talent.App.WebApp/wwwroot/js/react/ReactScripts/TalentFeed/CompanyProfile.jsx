import React from "react";
import Cookies from "js-cookie";
import { Loader } from "semantic-ui-react";
import { List, Card, Image, Icon } from "semantic-ui-react";

export default class CompanyProfile extends React.Component {
  constructor(props) {
    super(props);
    //let loader = loaderData;
    // loader.allowedUsers.push("Employer");
    // loader.allowedUsers.push("Recruiter");

    this.state = {
      profileData: {
        companyContact: {
          name: "",
          email: "",
          phone: "",
          location: {
            country: "",
            city: "",
          },
        },

        profilePhoto: null,
        profilePhotoUrl: null,
        skills: [],
      },

      loaderData: null,
      // loadingFeedData: false,
      //companyDetails: null,
    };

    this.init = this.init.bind(this);
    this.updateWithoutSave = this.updateWithoutSave.bind(this);
    this.loadData = this.loadData.bind(this);
  }

  init() {
    let loaderData = TalentUtil.deepCopy(this.state.loaderData);
    loaderData.isLoading = false;
    this.setState({ loaderData }); //comment this
  }

  componentDidMount() {
    //window.addEventListener('scroll', this.handleScroll);
    this.loadData();
    // this.init();
  }
  updateWithoutSave(newValues) {
    //let newCompany = Object.assign({}, this.state.profileData, newValues);
    this.setState({
      profileData: newValues,
    });
  }

  loadData() {
    var cookies = Cookies.get("talentAuthToken");
    $.ajax({
      url: "https://talent-standerd-module-01-pro.azurewebsites.net/profile/profile/getEmployerProfile",
      headers: {
        Authorization: "Bearer " + cookies,
        "Content-Type": "application/json",
      },
      type: "GET",
      contentType: "application/json",
      dataType: "json",
      success: function (res) {
        let employerData = null;
        if (res.employer) {
          employerData = res.employer;
          console.log("employerData", employerData);
        }
        this.updateWithoutSave(employerData);
      }.bind(this),
      error: function (res) {
        console.log(res.status);
      },
    });
    //this.init();
  }

  render() {
    const { companyContact, profilePhoto, profilePhotoUrl, skills } =
      this.state.profileData;
    console.log("Profile Date :", this.state.profileData);
    return (
      <Card>
        <Card.Content textAlign="center">
          <div>
            <Image
              src={
                profilePhotoUrl
                  ? `'${profilePhotoUrl}`
                  : "https://react.semantic-ui.com/images/wireframe/square-image.png"
              }
              circular
              size="tiny"
            />
            <div>
              <br />
            </div>
          </div>
          <Card.Header>{companyContact.name}</Card.Header>
          <Card.Meta>
            <span className="date">
              <Icon name="location arrow" />
              {companyContact.location.city}
              {", "}
              {companyContact.location.country}
            </span>
          </Card.Meta>
          <Card.Description>
            {skills
              ? "We Currently do not have specific skills that we desire."
              : skills}
          </Card.Description>
        </Card.Content>
        <Card.Content extra>
          <List>
            <List.Item icon="phone" content={":  " + companyContact.phone} />
            <List.Item icon="mail" content={":  " + companyContact.email} />
          </List>
        </Card.Content>
      </Card>
    );
  }
}
