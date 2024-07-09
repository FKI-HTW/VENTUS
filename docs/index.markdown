---
# Feel free to add content and custom Front Matter to this file.
# To modify the layout, see https://jekyllrb.com/docs/themes/#overriding-theme-defaults

layout: home
title: Home
nav_order: 1
---

# VENTUS
###### Virtual Environment for Teamwork and ad-hoc Collaboration between Companies and heterogeneous User Groups

<br/>
<span class="py-5 fs-5">
    <a type="button" name="button" class="btn btn-blue" href="#getting-started">
        Getting started
    </a>
    <a type="button" name="button" class="btn ml-2" href="https://github.com/FKI-HTW/VENTUS">
        View it on GitHub
    </a>
</span>

---

The aim of the VENTUS project is to develop a flexible and cost-effective system for the exchange and interactive visualization of 3D data and to publish it as an open source application. The visualization is made possible with head-mounted displays from various manufacturers as well as standard screens. To support collaboration, various interaction techniques such as pointing, gestures and the creation of annotations, 3D-sketching and CAD-models were implemented. The exchange of extensive model data between collaboration partners is simple and efficient. The system is geared towards business and product development processes in small and medium-sized enterprises (SMEs).

While VENTUS can be downloaded and used as a complete application, VENTUS is also meant to be used as a framework for future developments in VR. For this, features in VENTUS were made available as separate Unity packages, which can be imported into any other project.

## Getting Started

The newest version of the application can be downloaded for windows from <a href="https://github.com/FKI-HTW/VENTUS/releases">Releases</a>.
Alternatively the source code can also be downloaded or cloned from <a href="https://github.com/FKI-HTW/VENTUS">Github</a>. The downloaded project can then be opened with a supported Unity Editor version (2022.3.28+ as of 01.07.24) and built to a desired platform. The application starts in desktop-mode by default. VR-mode can be started from the taskbar once a supported HMD is connected and activated through an OpenXR runtime (SteamVR or Meta Quest Link is recommended).


VENTUS comes with six example library CAD-models, but allows importing of STEP- and GLTF-models using the file explorer. The model importer can be easily extended if other types of CAD-models are desired. Most features, like the file explorer, can be imported as Unity packages from the <a href="https://github.com/orgs/FKI-HTW/repositories">CENTIS Github</a>.

### User Documentation

In the wiki you can find documentation about using the VENTUS project and all its feautures. You can find the documentation at <a href="/VENTUS/user/">User Documentation</a>. These features are also explained within the VENTUS application itself as part of the help menu, where you can find both explanations and tutorial videos showcasing the implemented features.


### Developer Documentation

In the wiki you can find documentation about cloning and extending the VENTUS Unity project. You can find the documentation at <a href="/VENTUS/developer/">Developer Documentation</a>. The developer documentation also explains which features are provided as separate packages and where to find them.
